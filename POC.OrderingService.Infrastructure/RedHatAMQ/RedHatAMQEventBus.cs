using System.Collections.Concurrent;
using System.Text;
using Apache.NMS;
using Apache.NMS.AMQP;
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
        private readonly static ConcurrentDictionary<string, IMessageConsumer> _consumers;
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
            _publisher = publisher;
        }

        public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true) where T : class
        {
            if (onUnitOfWorkComplete && _unitOfWorkManager.Current is { } currentUow)
            {
                currentUow.OnCompleted(() => SaveToOutOfBox(eventData));
                return Task.CompletedTask;
            }

            return SaveToOutOfBox(eventData);
        }


        private async Task SaveToOutOfBox<T>(T eventData) where T : class
        {
            await _outboxManager.EnqueueAsync(new OutgoingEventInfo
                (
                id: Guid.NewGuid(),
                eventName: typeof(T).Name,
                eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                creationTime: DateTime.UtcNow)
                );
        }

        private async Task PublishToRedHatAMQOnUOWCompleteAsync<T>(T eventData) where T : class
        {
            if (_unitOfWorkManager.Current != null) {
                _unitOfWorkManager!.Current!.OnCompleted(async () =>
                {
                    await PublishMessage0(eventData);
                });
            }

        }

        private async Task PublishMessage0<T>(T eventData) where T : class
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
            var correlationId = _httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString();
            if (Guid.TryParse(correlationId, out var guid))
            {
                return guid;
            }
            return null;
        }


        public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where T : class
        {
            if (useOutbox)
            {
                var outGoingEvent = new OutgoingEventInfo(
                    id: Guid.NewGuid(),
                    eventName: typeof(T).GetFullNameWithAssemblyName(),
                    eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                    creationTime: DateTime.UtcNow);

                outGoingEvent.SetCorrelationId(GetCorrelationId()?.ToString() ?? string.Empty);

                return _outboxManager.EnqueueAsync(outGoingEvent);
            }
            else if (onUnitOfWorkComplete)
            {
                return PublishToRedHatAMQOnUOWCompleteAsync(eventData);

            }
            else
            {
                return PublishMessage0(eventData);
            }
        }



        public void Dispose()
        {
            _session?.Close();
            _connection?.Close();
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
        {
            var method = typeof(RedHatAMQEventBus)
                .GetMethod(nameof(PublishAsync), [eventType, typeof(bool), typeof(bool)])
                ?? throw new InvalidOperationException($"Could not find generic method PublishAsync<{eventType.Name}>");

            var genericMethod = method.MakeGenericMethod(eventType);

            return (Task)genericMethod.Invoke(this, [eventData, onUnitOfWorkComplete, useOutbox]);
        }


        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
        {
            return PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox: true);
        }


        public IDisposable Subscribe<TEvent, THandler>()
         where TEvent : class
         where THandler : IEventHandler, new()
        {
            throw new NotImplementedByDesignException();
        }

        public IDisposable Subscribe<T>(IDistributedEventHandler<T> handler) where T : class
        {
            throw new NotImplementedByDesignException();
        }

        public IDisposable Subscribe<T>(Func<T, Task> handler) where T : class
        {
            throw new NotImplementedByDesignException();
        }
        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedByDesignException();
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            throw new NotImplementedByDesignException();
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedByDesignException();

        }

        public async void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            throw new NotImplementedByDesignException();
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            throw new NotImplementedByDesignException();
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedByDesignException();
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            throw new NotImplementedByDesignException();
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedByDesignException();
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            throw new NotImplementedByDesignException();
        }

        public void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedByDesignException();
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
            var @event = this._jsonSerializer.Deserialize(type, eventStr, false);
            return @event;
        }
    }



}
