using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Abstraction.Dtos.Orders;

namespace POC.OrderingService.Query.Abstraction.Repositories
{
    public interface IOrderReadModelRepository
    {
        Task<IEnumerable<OrderReadModelDto>> GetCustomerOrders(string name);
    }
}
