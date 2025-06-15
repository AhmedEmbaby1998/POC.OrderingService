using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;
using POC.Orders;
using POC.Orders.Query;
using POC.Shared.ValueObjects;

namespace POC.Features.Orders.Mappers
{
    public static class OrderDtoMapper
    {
        public static OrderDto ToDto(this OrderReadModel order)
        {
            if (order == null) return null;
            return new OrderDto
            (
                 order.Id,
                 order.CustomerName,
                 new Address(order.Street,order.City,order.Street,order.ZipCode),
                 new Money(order.Amount,order.Currency),
                 order.Items?.Select(i => i.ToDto())?.ToList() ?? []
            );
        }
        private static OrderItemDto ToDto(this OrderItemReadModel item)
        {
            if (item == null) return null;

            return new OrderItemDto
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = new Quantity(item.Count, item.Unit),
                Money = new Money(item.Amount, item.Currency)
            };
        }
    }
}
