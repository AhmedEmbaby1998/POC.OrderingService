using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using POC.Abstractions;
using POC.EntityFrameworkCore;
using POC.Orders;
using POC.Orders.Events.DomainEvents;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Json;

namespace POC.Repositories.Orders
{

    public class OrderWriteRepository : WriteRepository<Order,OrderId>,IOrderRepository
    {
        public OrderWriteRepository(IEventStore eventStore, ILocalEventBus eventBus, IJsonSerializer jsonSerializer)
            : base(eventStore, eventBus,jsonSerializer)
        {
        }

        public override async Task<Order> GetAsync(OrderId orderId)
        {
            var historyEvents =await EventStore.GetEventsAsync(orderId.Value.ToString());
            List<EventSourcedEvent> events = [];
            foreach (var e in historyEvents)
            {
                var eventType = Type.GetType(e.EventType);
                var domainEvent = this.JsonSerializer.Deserialize(eventType, e.EventData);
                events.Add((EventSourcedEvent)domainEvent);
            }
            var order = Order.Rehydrate(events);
            return order;
        }
    }
}
