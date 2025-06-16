using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using POC.Abstractions;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events.EventsSourced
{
    internal record OrderCreatedEventSourced : EventSourcedEvent
    {
        [JsonConstructor]
        internal OrderCreatedEventSourced(OrderId orderId, string customerName,Address address,DateTimeOffset orderDate)
            : base(orderId.Value) 
        {
            OrderId = orderId;
            CustomerName = customerName;
            Address = address;
            OrderDate = orderDate;
        }

        public OrderId OrderId { get; set; }
        public string CustomerName { get; set; }
        public Address Address { get; set; }
        public DateTimeOffset OrderDate { get; set; }
    }
}
