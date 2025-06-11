using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;

namespace POC.Orders.IntegrationEvents
{
    public record class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public Address Address { get; set; }
        public DateOnly? DeliveryDate { set; get; }
        public Money TotalPrice { get; set; }
        public IEnumerable<OrderItemCreatedEvent> Items { get; set; } = new List<OrderItemCreatedEvent>();
    }


    public record class OrderItemCreatedEvent
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public Quantity Quantity { get; set; }
        public Money Price { get; set; }
    }
}
