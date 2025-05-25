using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Features.Orders.Mappers;
using POC.Orders;
using POC.Orders.Query;
using Volo.Abp.ObjectExtending;

namespace POC.Features.Orders.QueryHandlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
           return await _orderRepository.GetAsync(OrderId.New(request.OrderId))
                .ContinueWith(task => task.Result.ToDto(), cancellationToken);
        }
    }
}
