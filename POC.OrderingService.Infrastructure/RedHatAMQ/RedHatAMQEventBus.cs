using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace POC.OrderingService.Infrastructure.RedHatAMQ
{
    public record MyEvent99(string Message);

    internal class RedHatAMQEventBus : IDistributedEventBus
    {
        private readonly static ConcurrentDictionary<string, IMessageConsumer> _consumers ;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RedHatAMQEventBus> _logger;
        private readonly IEventOutboxManager _outboxManager;
        private readonly IJsonSerializer _jsonSerializer;
        private  IConnection _connection;
        private  Apache.NMS.ISession _session;
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
            IOptions<RedHatAMQSettings> options)
        {
            _logger = logger;
            _outboxManager = outboxManager;
            _jsonSerializer = jsonSerializer;
            _redhatSettings = options.Value;
            _unitOfWorkManager = unitOfWorkManager;
            _httpContextAccessor = httpContextAccessor;
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
                await PublishToRedHatAMQAfterUOWCompleteAsync(eventData);
            }
        }

        private async Task PublishToRedHatAMQAfterUOWCompleteAsync<T>(T eventData, string queueName = null) where T : class
        {
            if (_unitOfWorkManager.Current != null) {
                _unitOfWorkManager!.Current!.OnCompleted(async () =>
                {
                    await _publisher.Publish(eventData, ctx =>
                    {
                        ctx.Durable = true;
                        ctx.SetRoutingKey(queueName ?? $"queue.{typeof(T).Name}");
                        ctx.CorrelationId = GetCorrelationId();
                    }
                    );
                });
            }
                
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

        private async Task PublishToRedhatAsync<T>(T eventData, string queueName) where T : class
        {
            try
            {
                var eventName = typeof(T).Name;
                var destination = _session.GetQueue(queueName ?? $"queue.{eventName}");
                var producer = _session.CreateProducer(destination);

                var message = _session.CreateTextMessage(_jsonSerializer.Serialize(eventData));
                message.Properties["EventType"] = eventName;
                message.Properties["PublishedAt"] = DateTime.UtcNow.ToString("O");
                message.Properties["CorrelationId"] = _httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString()
                        ?? string.Empty;

                await Task.Run(() => producer.Send(message));

                _logger.LogInformation("Published event {EventType} to RedHatAMQ queue {Queue}",
                        eventName, destination.QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventType} to RedHatAMQ", typeof(T).Name);
                throw;
            }
        }

        // Other IDistributedEventBus methods...
        public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where T : class
        {
            if(useOutbox && onUnitOfWorkComplete)
            {
                return _outboxManager.EnqueueAsync(new OutgoingEventInfo(
                    id: Guid.NewGuid(),
                    eventName: typeof(T).Name,
                    eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                    creationTime: DateTime.UtcNow));
            }

            return PublishToRedHatAMQAfterUOWCompleteAsync(eventData);
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

            return PublishToRedHatAMQAfterUOWCompleteAsync(eventData);

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
