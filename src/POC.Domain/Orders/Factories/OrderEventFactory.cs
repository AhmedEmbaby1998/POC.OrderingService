using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Orders.Events.DomainEvents;

namespace POC.Orders.Factories
{
    internal static class OrderEventFactory
    {
        internal static OrderEvent ToOrderCreatedEvent(this Order order)
        {
            return new OrderEvent
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalPrice = order.TotalPrice,
                Address = order.Address,
                DeliveryDate = order.DeliveryDate,
                Items = order.Items.Select(item => new OrderItemEvent
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }),
                EventType = OrderEventType.Created
            };
        }

        internal static OrderEvent ToOrderPaidEvent(this Order order)
        {
            return new OrderEvent
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalPrice = order.TotalPrice,
                Address = order.Address,
                DeliveryDate = order.DeliveryDate,
                Items = order.Items.Select(item => new OrderItemEvent
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }),
                EventType = OrderEventType.Paid
            };
        }
    }
}
