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
        private readonly IMediator mediator;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
        {
            _orderRepository = orderRepository;
            this.mediator = mediator;
        }

        [UnitOfWork]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(OrderId.New(), request.CustomerName, request.Address);
            //order.SetItems(request.OrderItems.Select(d => new POC.Orders.OrderItem(d.ProductName, d.Quantity, d.Money)));
            await _orderRepository.SaveAsync(order, cancellationToken);
            //foreach(var e in order.GetLocalEvents())
            //{
            //    await mediator.Publish(e.EventData, cancellationToken);
            //}
            return order.Id;
        }
    }
}
