using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Orders;
using POC.Orders.Commands;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace POC.Features.Orders.CommandHandlers
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICorrelationIdProvider _correlationIdProvider;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, ICorrelationIdProvider correlationIdProvider)
        {
            _orderRepository = orderRepository;
            _correlationIdProvider = correlationIdProvider;
        }

        [UnitOfWork]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(OrderId.New(), request.CustomerName, request.Address);
            order.SetItems(request.OrderItems.Select(i => new POC.Orders.OrderItem(i.ProductName, i.Quantity, i.Money)));
            await _orderRepository.SaveAsync(order, cancellationToken);
            return order.Id.Value;
        }
    }
}
