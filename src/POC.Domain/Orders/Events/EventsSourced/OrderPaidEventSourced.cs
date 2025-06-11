using System;
using System.Text.Json.Serialization;
using POC.Abstractions;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events.EventsSourced
{
    public record OrderPaidEventSourced : EventSourcedEvent
    {
        [JsonConstructor]
        public OrderPaidEventSourced(OrderId orderId, string customerName, Money totalPrice)
            : base(orderId)
        {
            OrderId = orderId;
            CustomerName = customerName;
            TotalPrice = totalPrice;
        }

        public OrderId OrderId { get; }
        public string CustomerName { get; }
        public Money TotalPrice { get; }
    }
}
