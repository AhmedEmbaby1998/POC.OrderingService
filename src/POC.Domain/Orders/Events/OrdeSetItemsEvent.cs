using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POC.Orders.Events
{
    public record OrdeSetItemsEvent
    {
        public OrderId OrderId { get; }
        public IEnumerable<OrderItem> Items { get; }

        [JsonConstructor]
        public OrdeSetItemsEvent(OrderId orderId, IEnumerable<OrderItem> items)
        {
            OrderId = orderId;
            Items = items;
        }
    }
}
