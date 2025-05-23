using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events
{
    public record OrderCreatedEvent
    {
        public OrderCreatedEvent(OrderId orderId, string customerName, DateTime orderDate,Address address)
        {
            OrderId = orderId;
            CustomerName = customerName;
            OrderDate = orderDate;
        }

        public OrderId OrderId { get; }
        public Address ShippingAddress { get; }
        public string CustomerName { get; }
        public DateTime OrderDate { get; }
    }
}
