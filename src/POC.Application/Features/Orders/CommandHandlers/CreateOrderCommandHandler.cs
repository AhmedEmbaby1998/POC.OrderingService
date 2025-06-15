using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Orders;
using POC.Orders.Commands;
using Volo.Abp.Uow;

namespace POC.Features.Orders.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [UnitOfWork]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(OrderId.New(), request.CustomerName, request.Address);
            await _orderRepository.SaveAsync(order, cancellationToken);
            return order.Id;
        }
    }
}
