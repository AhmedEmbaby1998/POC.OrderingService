using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using POC.Shared.ValueObjects;

namespace POC.Orders.Commands
{
    public record class OrderItem(string ProductName, Quantity Quantity,Money Money);
    public record CreateOrderCommand : IRequest<Guid>
    {
        public CreateOrderCommand(string customerName, IEnumerable<OrderItem> orderItems, Address address)
        {
            CustomerName = customerName;
            OrderItems = orderItems;
            Address = address;
        }

        public string CustomerName { get; }
        public IEnumerable<OrderItem> OrderItems { get; }
        public Address Address { get; }
    }
}
