using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using POC.Abstractions;

namespace POC.Orders.Events.EventsSourced
{
    internal record OrdeSetItemsEventSourced : EventSourcedEvent,INotification
    {
        public OrderId OrderId { get; }
        public IEnumerable<OrderItem> Items { get; }

        [JsonConstructor]
        internal OrdeSetItemsEventSourced(OrderId orderId, IEnumerable<OrderItem> items)
            : base(orderId)
        {
            OrderId = orderId;
            Items = items;
        }
    }
}
