using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Orders;
using POC.Orders.Commands;
using POC.Orders.IntegrationEvents;
using POC.Shared.ValueObjects;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace POC.Features.Orders.CommandHandlers
{
    internal class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDistributedEventBus _distributedEventBus;
        public PayOrderCommandHandler(IOrderRepository orderRepository, IDistributedEventBus distributedEventBus)
        {
            _orderRepository = orderRepository;
            _distributedEventBus = distributedEventBus;
        }


        [UnitOfWork]
        public async Task<Guid> Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(OrderId.New(request.Id));
            order.Pay(request.TotalPrice);
            await _orderRepository.SaveAsync(order, cancellationToken);
            await _distributedEventBus.PublishAsync(eventData: new OrderPaidETo66(order.Id.Value, order.TotalPrice), onUnitOfWorkComplete: true, useOutbox: true);
            return order.Id.Value;
        }
    }
}
