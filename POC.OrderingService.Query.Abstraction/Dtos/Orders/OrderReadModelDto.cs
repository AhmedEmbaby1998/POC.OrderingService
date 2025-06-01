using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POC.OrderingService.Query.Abstraction.Dtos.Orders
{
    public class OrderReadModelDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get;set; }
        public string Street { get;  set; }
        public string City { get;  set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public DateOnly? DeliveryDate { set; get; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public IEnumerable<OrderItemReadModelDto> Items { set; get; }
    }
}
