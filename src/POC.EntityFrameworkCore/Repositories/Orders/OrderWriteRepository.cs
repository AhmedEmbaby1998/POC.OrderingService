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

        public override Order ReHydrate(IEnumerable<EventSourcedEvent> events)
        {
            return Order.Rehydrate(events);
        }
    }
}
