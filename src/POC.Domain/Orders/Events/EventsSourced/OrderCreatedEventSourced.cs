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
        internal OrderCreatedEventSourced(OrderId orderId, string customerName, DateTimeOffset orderDate)
            : base(orderId)
        {
            OrderId = orderId;
            CustomerName = customerName;
            OrderDate = orderDate;
        }

        public OrderId OrderId { get; }
        public string CustomerName { get; }
        public DateTimeOffset OrderDate { get; }
    }
}
