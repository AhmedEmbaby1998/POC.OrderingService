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
using Volo.Abp.Tracing;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Http;

namespace POC.Repositories.Orders
{

    public class OrderWriteRepository : WriteRepository<Order,OrderId>,IOrderRepository
    {
        public OrderWriteRepository(IEventStore eventStore, ILocalEventBus eventBus, IJsonSerializer jsonSerializer, IHttpContextAccessor contextAccessor, ICurrentUser currentUser)
            : base(eventStore, eventBus,jsonSerializer,currentUser, contextAccessor)
        {
        }

        public override async Task<Order> GetAsync(OrderId orderId)
        {
            var historyEvents =await EventStore.GetEventsAsync(orderId.Value.ToString());
            List<EventSourcedEvent> eventSourcedEvents = [];
            foreach (var @event in historyEvents)
            {
                var eventType = Type.GetType(@event.EventType);
                var domainEvent = this.JsonSerializer.Deserialize(eventType, @event.EventData);
                eventSourcedEvents.Add((EventSourcedEvent)domainEvent);
            }
            var order = Order.Rehydrate(eventSourcedEvents);
            return order;
        }
    }
}
