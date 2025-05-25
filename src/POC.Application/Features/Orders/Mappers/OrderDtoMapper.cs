using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Orders;
using POC.Orders.Query;

namespace POC.Features.Orders.Mappers
{
    public static class OrderDtoMapper
    {
        public static OrderDto ToDto(this Order order)
        {
            if (order == null) return null;
            return new OrderDto
            (
                 order.Id,
                 order.CustomerName,
                 order.Address,
                 order.TotalPrice,
                 order.Items?.Select(i => i.ToDto()).ToList()
            );
        }
        private static OrderItemDto ToDto(this OrderItem item)
        {
            if (item == null) return null;
            return new OrderItemDto
            (
                item.ProductName,
                item.Quantity,
                item.Price
            );
        }
    }
}
