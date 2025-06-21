using System.Collections.Concurrent;
using System.Text;
using Apache.NMS;
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

namespace POC.OrderingService.Infrastructure.RedHatAMQ;

internal class RedHatAMQEventBus : IDistributedEventBus, ISupportsEventBoxes
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RedHatAMQEventBus> _logger;
    private readonly IEventOutboxManager _outboxManager;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IPublishEndpoint _publisher;

    public RedHatAMQEventBus(
        ILogger<RedHatAMQEventBus> logger,
        IEventOutboxManager outboxManager,
        IJsonSerializer jsonSerializer,
        IUnitOfWorkManager unitOfWorkManager,
        IHttpContextAccessor httpContextAccessor,
        IPublishEndpoint publisher)
    {
        _logger = logger;
        _outboxManager = outboxManager;
        _jsonSerializer = jsonSerializer;
        _unitOfWorkManager = unitOfWorkManager;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
    }

    // -------------------- Publish --------------------

    public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where T : class
    {
        if (useOutbox)
            return EnqueueToOutbox(eventData);

        if (onUnitOfWorkComplete && _unitOfWorkManager.Current is { } uow)
        {
            uow.OnCompleted(() => PublishToBroker(eventData));
            return Task.CompletedTask;
        }

        return PublishToBroker(eventData);
    }

    public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true) where T : class =>
        PublishAsync(eventData, onUnitOfWorkComplete, useOutbox: true);

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
    {
        var method = typeof(RedHatAMQEventBus)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == nameof(PublishAsync) &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 3) ?? throw new InvalidOperationException($"Could not find method PublishAsync<T>");

        var genericMethod = method.MakeGenericMethod(eventType);
        return (Task)genericMethod.Invoke(this, new[] { eventData, onUnitOfWorkComplete, useOutbox })!;
    }

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true) =>
        PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox: true);

    // -------------------- Subscribe (Not Implemented) --------------------

    public IDisposable Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler, new() =>
        throw new NotImplementedByDesignException();

    public IDisposable Subscribe<T>(IDistributedEventHandler<T> handler) where T : class =>
        throw new NotImplementedByDesignException();

    public IDisposable Subscribe<T>(Func<T, Task> handler) where T : class =>
        throw new NotImplementedByDesignException();

    public IDisposable Subscribe(Type eventType, IEventHandler handler) =>
        throw new NotImplementedByDesignException();

    public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class =>
        throw new NotImplementedByDesignException();

    public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory) =>
        throw new NotImplementedByDesignException();

    // -------------------- Unsubscribe (Not Implemented) --------------------

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class =>
        throw new NotImplementedByDesignException();

    public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class =>
        throw new NotImplementedByDesignException();

    public void Unsubscribe(Type eventType, IEventHandler handler) =>
        throw new NotImplementedByDesignException();

    public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class =>
        throw new NotImplementedByDesignException();

    public void Unsubscribe(Type eventType, IEventHandlerFactory factory) =>
        throw new NotImplementedByDesignException();

    public void UnsubscribeAll<TEvent>() where TEvent : class =>
        throw new NotImplementedByDesignException();

    public void UnsubscribeAll(Type eventType) =>
        throw new NotImplementedByDesignException();

    // -------------------- Outbox/InOutbox --------------------

    public async Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
    {
        ArgumentNullException.ThrowIfNull(outgoingEvent);
        ArgumentNullException.ThrowIfNull(outboxConfig);

        var @event = DeserializeEvent(outgoingEvent.EventName, outgoingEvent.EventData)
            ?? throw new InvalidOperationException("Deserialization returned null");

        await PublishToBroker((dynamic)@event);
    }
    public async Task PublishManyFromOutboxAsync(IEnumerable<OutgoingEventInfo> events, OutboxConfig config)
    {
        foreach (var e in events)
        {
            await PublishFromOutboxAsync(e, config);
        }
    }
    public Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig) =>
        throw new NotImplementedByDesignException("Inbox pattern is not supported yet.");

    #region Private Helpers
    private Task EnqueueToOutbox<T>(T eventData) where T : class
    {
        var @event = new OutgoingEventInfo(
            id: Guid.NewGuid(),
            eventName: typeof(T).GetFullNameWithAssemblyName(),
            eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
            creationTime: DateTime.UtcNow);

        @event.SetCorrelationId(GetCorrelationId()?.ToString() ?? string.Empty);

        return _outboxManager.EnqueueAsync(@event);
    }
    private Task PublishToBroker<T>(T eventData) where T : class =>
        _publisher.Publish(eventData, ctx =>
        {
            ctx.Durable = true;
            ctx.CorrelationId = GetCorrelationId();
        });
    private object? DeserializeEvent(string eventType, byte[] eventBytes)
    {
        var type = Type.GetType(eventType) ?? throw new InvalidOperationException($"Type {eventType} not found.");
        var json = Encoding.UTF8.GetString(eventBytes);
        return _jsonSerializer.Deserialize(type, json, false);
    }
    private Guid? GetCorrelationId()
    {
        var correlation = _httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString();
        return Guid.TryParse(correlation, out var guid) ? guid : null;
    }
    #endregion Private Helpers
}
