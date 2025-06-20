using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amqp.Handler;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POC.OrderingService.Infrastructure.ActiveMq;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace POC.OrderingService.Infrastructure.RedHatAMQ
{
    internal class RedHatAMQEventBus : IDistributedEventBus, ISupportsEventBoxes
    {
        private readonly static ConcurrentDictionary<string, IMessageConsumer> _consumers ;
        private readonly static string QUEUE_NAME_PREFIX = "ordering.";
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RedHatAMQEventBus> _logger;
        private readonly IEventOutboxManager _outboxManager;
        private readonly IJsonSerializer _jsonSerializer;
        private IConnection _connection;
        private Apache.NMS.ISession _session;
        private readonly RedHatAMQSettings _redhatSettings;
        private readonly IPublishEndpoint _publisher;
        static RedHatAMQEventBus()
        { 
            _consumers = new ConcurrentDictionary<string, IMessageConsumer>(); 
        }
        public RedHatAMQEventBus(
            ILogger<RedHatAMQEventBus> logger,
            IEventOutboxManager outboxManager,
            IJsonSerializer jsonSerializer,
            IUnitOfWorkManager unitOfWorkManager,
            IHttpContextAccessor httpContextAccessor,
            IOptions<RedHatAMQSettings> options,
            IPublishEndpoint publisher)
        {
            _logger = logger;
            _outboxManager = outboxManager;
            _jsonSerializer = jsonSerializer;
            _redhatSettings = options.Value;
            _unitOfWorkManager = unitOfWorkManager;
            _httpContextAccessor = httpContextAccessor;
            _publisher =publisher;
            InitializeConnection();

        }

        private void InitializeConnection()
        {
            
            // var f = new ConnectionFactory(_redhatSettings.BrokerUri);
            //_connection = f.CreateConnection(_redhatSettings.UserName,_redhatSettings.Password);
            //   _connection.Start();
            //  _session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        }

        public async Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true) where T : class
        {
            if (onUnitOfWorkComplete)
            {
                // Store in outbox - will be processed after UOW commits
                await _outboxManager.EnqueueAsync(new OutgoingEventInfo
                    (
                    id: Guid.NewGuid(),
                    eventName: typeof(T).Name,
                    eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                    creationTime: DateTime.UtcNow)
                    );
            }
            else
            {
                // Publish immediately
                await PublishToRedHatAMQAfterUOWCompleteAsync(eventData,eventData.GetTopicName());
            }
        }

        private async Task PublishToRedHatAMQAfterUOWCompleteAsync<T>(T eventData, string queueName) where T : class
        {
            if (_unitOfWorkManager.Current != null) {
                _unitOfWorkManager!.Current!.OnCompleted(async () =>
                {
                    await PublishMessage0(eventData, queueName);
                });
            }
                
        }

        private async Task PublishMessage0<T>(T eventData, string queueName) where T : class 
        {
            await _publisher.Publish(eventData, ctx =>
            {
                ctx.Durable = true;
                //it will publish to a Topic so it will not be End to End.
                ctx.CorrelationId = GetCorrelationId();
            }
            );
        }

        private Guid? GetCorrelationId()
        {
            var correlationId=_httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString();
            if (Guid.TryParse(correlationId, out var guid))
            {
                return guid;
            }
            return null;
        }


        // Other IDistributedEventBus methods
        public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where T : class
        {
            if(useOutbox && onUnitOfWorkComplete)
            {
                var outGoingEvent = new OutgoingEventInfo(
                    id: Guid.NewGuid(),
                    eventName: typeof(T).GetFullNameWithAssemblyName(),
                    eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                    creationTime: DateTime.UtcNow); 

                outGoingEvent.SetCorrelationId(GetCorrelationId()?.ToString() ?? string.Empty);

                return _outboxManager.EnqueueAsync(outGoingEvent);
            }

            return PublishToRedHatAMQAfterUOWCompleteAsync(eventData,eventData.GetTopicName());
        }

       

        public void Dispose()
        {
            _session?.Close();
            _connection?.Close();
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
        {
            if (useOutbox && onUnitOfWorkComplete)
            {
                var method = typeof(RedHatAMQEventBus)
                .GetMethod(nameof(PublishAsync), [eventType, typeof(bool)]);

                var genericMethod = method?.MakeGenericMethod(eventType);
                return (Task)genericMethod?.Invoke(this, [eventData, onUnitOfWorkComplete]);
            }

            return PublishToRedHatAMQAfterUOWCompleteAsync(eventData, eventData.GetTopicName());

        }

        public async Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
        {
             await PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox: true);
        }

        public IDisposable Subscribe<TEvent, THandler>()
         where TEvent : class
         where THandler : IEventHandler, new()
        {
            var handler = new THandler();

            if (handler is IDistributedEventHandler<TEvent> distributedHandler)
            {
                return Subscribe(distributedHandler);
            }

            throw new ArgumentException($"Handler {typeof(THandler)} must implement IDistributedEventHandler<{typeof(TEvent)}>");
        }

        public IDisposable Subscribe<T>(IDistributedEventHandler<T> handler) where T : class
        {
            // Implementation for subscribing to events
            var eventName = typeof(T).Name;
            if(_consumers.ContainsKey(eventName))
            {
                _logger.LogWarning("Consumer for event {EventType} already exists. Reuse existing consumer.", eventName);
                return new DisposeAction(() => { });
            }
            var queue = _session.GetQueue($"queue.{eventName}");
            var consumer = _session.CreateConsumer(queue);

            consumer.Listener += async message =>
            {
                if (message is ITextMessage textMessage)
                {
                    try
                    {
                        var eventData = _jsonSerializer.Deserialize<T>(textMessage.Text);
                        await handler.HandleEventAsync(eventData);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling event {EventType}", eventName);
                    }
                }
            };

            _consumers.TryAdd(eventName, consumer);
            _logger.LogInformation("Subscribed to event {EventType} with consumer {ConsumerId}", eventName, consumer.ToString());
            return new DisposeAction(consumer.Close);
        }

        public IDisposable Subscribe<T>(Func<T, Task> handler) where T : class
        {
            return Subscribe(new FuncEventHandler<T>(handler));
        }
        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            var method = typeof(RedHatAMQEventBus)
                .GetMethod(nameof(Subscribe), new[] { typeof(IDistributedEventHandler<>).MakeGenericType(eventType) });

            return method == null
                ? throw new InvalidOperationException($"No suitable Subscribe method found for type {eventType.Name}")
                : (IDisposable)method.Invoke(this, [handler]);
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            var handler = factory.GetHandler();
            if (handler is IDistributedEventHandler<TEvent> distributedHandler)
            {
                return Subscribe(distributedHandler);
            }

            throw new ArgumentException($"Handler {handler.GetType()} must implement IDistributedEventHandler<{typeof(TEvent)}>");
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            var handler = factory.GetHandler();
            return Subscribe(eventType, handler: (IEventHandler)handler);
        }

        private async Task UnsubscribeInternalAsync(string queueName)
        {
            if (_consumers.TryGetValue(queueName, out var consumer))
            {
                try
                {
                    await consumer.CloseAsync();
                    consumer.Dispose();
                    _logger.LogInformation("Unsubscribed from RedHatAMQ queue {Queue}", queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while unsubscribing from {Queue}", queueName);
                }

                _consumers.Remove(queueName, out _);
            }
            else
                _logger.LogError("already unsubscribed to {QueueName}",queueName);
            
        }

        public async void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            var eventName = typeof(TEvent).Name;
            await UnsubscribeInternalAsync(eventName);
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            var eventName = typeof(TEvent).Name;
            UnsubscribeInternalAsync(eventName).GetAwaiter().GetResult();
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            var eventName = eventType.Name;
            UnsubscribeInternalAsync(eventName).GetAwaiter().GetResult();
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            var eventName = typeof(TEvent).Name;
            UnsubscribeInternalAsync(eventName).GetAwaiter().GetResult();
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            var eventName = eventType.Name;
            UnsubscribeInternalAsync(eventName).GetAwaiter().GetResult();
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedException();
        }

        public async Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
        {
            ArgumentNullException.ThrowIfNull(outgoingEvent);
            ArgumentNullException.ThrowIfNull(outboxConfig);

            // Deserialize the event data
            var @event = DeserializeEvent(outgoingEvent.EventName, outgoingEvent.EventData);
            ArgumentNullException.ThrowIfNull(@event);

            await this.PublishMessage0(
                eventData: (dynamic)@event
                ,
                queueName: outgoingEvent.EventName.GetTopicName()
            );
        }

        public async Task PublishManyFromOutboxAsync(IEnumerable<OutgoingEventInfo> outgoingEvents, OutboxConfig outboxConfig)
        {
            foreach (var outgoingEvent in outgoingEvents)
            {
                await PublishFromOutboxAsync(outgoingEvent, outboxConfig);
            }
        }
        public async Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig)
        {
            throw new NotImplementedByDesignException("We does not support InBox Pattern Yet!");
        }
        private static string HexToString(string hexString)
        {
            if (hexString.StartsWith("0x"))
                hexString = hexString.Substring(2);

            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }
        private object? DeserializeEvent(string eventType, byte[] byteEventData)
        {
            // Get the type from the assembly
            Type type = Type.GetType(eventType) ?? throw new InvalidOperationException($"Type {eventType} not found");
            //convert byte array to string
            var eventStr = Encoding.UTF8.GetString(byteEventData);
            // Deserialize the JSON to the specified type
            var @event= this._jsonSerializer.Deserialize(type, eventStr,false);
            return @event;
        }
    }

    // Helper class for function-based event handlers
    public class FuncEventHandler<T> : IDistributedEventHandler<T> where T : class
    {
        private readonly Func<T, Task> _handler;

        public FuncEventHandler(Func<T, Task> handler)
        {
            _handler = handler;
        }

        public Task HandleEventAsync(T eventData)
        {
            return _handler(eventData);
        }
    }
}
