using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Abstractions;
using Volo.Abp.Domain.Repositories;

namespace POC.Orders
{
    public interface IOrderRepository : IEventSourcingRepository<Order,OrderId>
    {

    }
}
