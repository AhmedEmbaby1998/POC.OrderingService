using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events
{
    public record OrderCreatedEvent
    {
        [JsonConstructor]
        public OrderCreatedEvent(OrderId orderId, string customerName, DateTime orderDate)
        {
            OrderId = orderId;
            CustomerName = customerName;
            OrderDate = orderDate;
        }

        public OrderId OrderId { get; }
        public string CustomerName { get; }
        public DateTime OrderDate { get; }
    }
}
