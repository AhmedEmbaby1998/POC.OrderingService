using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Orders;
using POC.Orders.Commands;
using Volo.Abp.Uow;

namespace POC.Features.Orders.CommandHandlers
{
    internal class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        public PayOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [UnitOfWork]
        public async Task<Guid> Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(OrderId.New(request.Id));
            order.Pay(request.TotalPrice);
            await _orderRepository.SaveAsync(order, cancellationToken);
            return order.Id.Value;
        }
    }
}
