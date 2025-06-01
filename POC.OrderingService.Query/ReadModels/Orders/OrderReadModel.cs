using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POC.OrderingService.Query.ReadModels.Orders
{
    public class OrderReadModel
    {
        public Guid Id { get;private set; }
        public string CustomerName { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public DateOnly? DeliveryDate { private set; get; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public IEnumerable<OrderItemReadModel> Items {private set; get; }
    }
}
