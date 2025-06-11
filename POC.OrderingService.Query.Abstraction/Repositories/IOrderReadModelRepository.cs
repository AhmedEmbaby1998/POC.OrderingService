using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Abstraction.Dtos.Orders;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;

namespace POC.OrderingService.Query.Abstraction.Repositories
{
    public interface IOrderReadModelRepository : IReadModelRepository<OrderReadModel>
    {
        Task<IEnumerable<OrderReadModelDto>> GetCustomerOrders(string customerName);
    }
}
