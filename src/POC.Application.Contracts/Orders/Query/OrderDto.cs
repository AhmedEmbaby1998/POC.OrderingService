using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Orders.Commands;
using POC.Shared.ValueObjects;

namespace POC.Orders.Query
{
    public record class OrderItemDto(string ProductName, Quantity Quantity, Money Money);

    public record OrderDto
    {
        public Guid Id { private set; get; }
        public string CustomerName { get; private set; }
        public Address Address { get; private set; }
        public Money TotalPrice { get; private set; }

        public IEnumerable<OrderItemDto> Items = [];

        public OrderDto(Guid id, string customerName, Address address, Money totalPrice, IEnumerable<OrderItemDto> items)
        {
            Id = id;
            CustomerName = customerName;
            Address = address;
            TotalPrice = totalPrice;
            Items = items;
        }


    }
}
