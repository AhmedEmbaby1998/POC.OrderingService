using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Orders.Commands;
using POC.Shared.ValueObjects;

namespace POC.Orders.Query
{
    public  class OrderItemDto
    {
        public Guid Id { init; get; }
        public string ProductName { init; get; }
        public Quantity Quantity { init; get; }
        public Money Money { init; get; }
    }

    public class OrderDto
    {
        public Guid Id {  set; get; }
        public string CustomerName { get;  set; }
        public Address Address { get;  set; }
        public Money TotalPrice { get;  set; }

        public IEnumerable<OrderItemDto> Items { set; get; }

        public OrderDto() { }
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
