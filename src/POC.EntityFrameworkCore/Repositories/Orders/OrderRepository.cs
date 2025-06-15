using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.EntityFrameworkCore;
using POC.Orders;
using POC.Orders.Events.DomainEvents;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Local;

namespace POC.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IEventStore _eventStore;
        private readonly ILocalEventBus _eventBus;
        public OrderRepository(IEventStore eventStore, ILocalEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task<Order> GetAsync(OrderId id)
        {
            var history =await _eventStore.GetEventsAsync(id);
            var order = Order.Rehydrate(history);
            return order;
        }

        public async Task SaveAsync(Order aggregate,CancellationToken cancellationToken)
        {
            var events = aggregate.UncommittedEvents.Select(e => new StoredEvent
            (
                eventType: e.GetType()?.FullName!,
                eventData: JsonSerializer.Serialize(e),
                createdAt: e.OccurredOn,
                aggregateId: e.AggregateId.ToString())
            );
            await _eventStore.SaveEventAsync(events, cancellationToken);
            aggregate.ClearUncommittedEvents();
            foreach (var e in aggregate.GetLocalEvents())
            {
                await _eventBus.PublishAsync((dynamic)e.EventData);
            }
            aggregate.ClearLocalEvents();
        }
    }
}
