using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public Task<Order> GetAsync(OrderId id)
        {
            var history = _eventStore.GetEventsAsync(id);
            var order = new Order();
            foreach (var @event in history.Result)
            {
                order.ApplyEvent(@event);
            }
        }

        public Task SaveAsync(Order aggregate)
        {
            throw new NotImplementedException();
        }
    }
}
