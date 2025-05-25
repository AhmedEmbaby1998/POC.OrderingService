using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Orders;
using POC.Orders.Commands;

namespace POC.Features.Orders.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(OrderId.New(), request.CustomerName, request.Address);
            order.SetItems(request.OrderItems.Select(d => new POC.Orders.OrderItem(d.ProductName, d.Quantity, d.Money)));
            await _orderRepository.SaveAsync(order, cancellationToken);
            return order.Id;
        }
    }
}
