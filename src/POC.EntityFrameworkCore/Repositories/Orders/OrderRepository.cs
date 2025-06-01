using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using POC.EntityFrameworkCore;
using POC.Orders;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace POC.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IEventStore _eventStore;

        public OrderRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Order> GetAsync(OrderId id)
        {
            var history =await _eventStore.GetEventsAsync(id);
            var order = Order.ApplyEvent(history);
            return order;
        }

        public async Task SaveAsync(Order aggregate,CancellationToken cancellationToken)
        {
            var events = aggregate.UncommittedEvents.Select(e => new StoredEvent
            {
                AggregateId = aggregate.Id.ToString(),
                EventType = e.GetType().FullName,
                EventData = JsonSerializer.Serialize(e),
                CreatedAt = DateTime.UtcNow
            });
            await _eventStore.SaveEventAsync(events, cancellationToken);
        }
    }
}
