using System;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events
{
    public record OrderPayedEvent
    {
        public OrderPayedEvent(OrderId orderId, string customerName, DateTime orderDate, Money totalPrice)
        {
            OrderId = orderId;
            CustomerName = customerName;
            OrderDate = orderDate;
            TotalPrice = totalPrice;
        }

        public OrderId OrderId { get; }
        public string CustomerName { get; }
        public DateTime OrderDate { get; }
        public Money TotalPrice { get; set; }
    }
}
