using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using POC.Abstractions;
using POC.Orders;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Json;

namespace POC.Repositories
{
    public abstract class WriteRepository<TAggregate, TId>
        where TAggregate : EventSourcingAggregateRoot<TId>
    {
        protected readonly IEventStore EventStore;
        protected readonly ILocalEventBus EventBus;
        protected readonly IJsonSerializer JsonSerializer;

        protected WriteRepository(IEventStore eventStore, ILocalEventBus eventBus, IJsonSerializer jsonSerializer)
        {
            EventStore = eventStore;
            EventBus = eventBus;
            JsonSerializer = jsonSerializer;
        }

        public abstract TAggregate ReHydrate(IEnumerable<EventSourcedEvent> events);

        public async Task<TAggregate> GetAsync(OrderId orderId)
        {
            var historyEvents = await EventStore.GetEventsAsync(orderId.Value.ToString());
            List<EventSourcedEvent> eventSourcedEvents = [];
            foreach (var @event in historyEvents)
            {
                var eventType = Type.GetType(@event.EventType);
                var domainEvent = this.JsonSerializer.Deserialize(eventType, @event.EventData);
                eventSourcedEvents.Add((EventSourcedEvent)domainEvent);
            }
            var agg = ReHydrate(eventSourcedEvents);
            return agg;
        }
        public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken)
        {
            var events = aggregate.UncommittedEvents.Select(e => new StoredEvent
            (
                eventType: e.GetType().AssemblyQualifiedName!,
                eventData: JsonSerializer.Serialize(e),
                createdAt: e.OccurredOn,
                aggregateId: e.AggregateId.ToString())
            );
            await EventStore.SaveEventAsync(events, cancellationToken);
            aggregate.ClearUncommittedEvents();
            foreach (var e in aggregate.GetLocalEvents())
            {
                await EventBus.PublishAsync((dynamic)e.EventData);
            }
            aggregate.ClearLocalEvents();
        }
    }
}
