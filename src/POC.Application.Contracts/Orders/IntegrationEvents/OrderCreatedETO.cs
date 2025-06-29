using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;
using Volo.Abp.EventBus;

namespace POC.Orders.IntegrationEvents
{
    [EventName("OrderCreated")]
    public record class OrderCreatedETO
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public Address Address { get; set; }
        public DateOnly? DeliveryDate { set; get; }
        public Money TotalPrice { get; set; }
        public IEnumerable<OrderItemCreatedETO> Items { get; set; } = new List<OrderItemCreatedETO>();
    }


    public record class OrderItemCreatedETO
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public Quantity Quantity { get; set; }
        public Money Price { get; set; }
    }
}
