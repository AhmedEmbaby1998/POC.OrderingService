using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using POC.Shared.ValueObjects;

namespace POC.Orders.Events.DomainEvents
{
    public class OrderEvent:INotification
    {
        public Guid Id { get; init; }
        public string CustomerName { get; init; }
        public Address Address { get; init; }
        public DateOnly? DeliveryDate { init; get; }
        public Money TotalPrice { get; init; }
        public OrderEventType EventType { get; init; }
        public IEnumerable<OrderItemEvent> Items { get; init; } = [];
    }

    public class OrderItemEvent
    {
        public Guid Id { get; init; }
        public string ProductName { get; init; }
        public Quantity Quantity { get; init; }
        public Money Price { get; init; }
    }
}
