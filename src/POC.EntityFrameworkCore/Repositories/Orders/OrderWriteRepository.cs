using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using POC.EntityFrameworkCore;
using POC.Orders;
using POC.Orders.Events.DomainEvents;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Local;

namespace POC.Repositories.Orders
{

    public class OrderWriteRepository : WriteRepository<Order,OrderId>,IOrderRepository
    {
        public OrderWriteRepository(IEventStore eventStore, ILocalEventBus eventBus)
            :base(eventStore, eventBus)
        {

        }

        public override async Task<Order> GetAsync(OrderId id)
        {
            var history =await EventStore.GetEventsAsync(id);
            var order = Order.Rehydrate(history);
            return order;
        }
    }
}
