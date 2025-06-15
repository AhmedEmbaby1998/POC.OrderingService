using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.Features.Orders.Mappers;
using POC.OrderingService.Query.Abstraction.Dtos.Orders;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.Orders;
using POC.Orders.Query;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;

namespace POC.Features.Orders.QueryHandlers
{
    public class GetOrdersByCustomerNameQueryHandler : IRequestHandler<GetOrderByCustomerNameQuery,IEnumerable<OrderDto>>
    {
        private readonly IOrderReadModelRepository _orderRepository;
        private readonly IObjectMapper _objectMapper;
        public GetOrdersByCustomerNameQueryHandler(IOrderReadModelRepository orderRepository, IObjectMapper objectMapper)
        {
            _orderRepository = orderRepository;
            _objectMapper = objectMapper;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrderByCustomerNameQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<OrderReadModelDto> orders = await _orderRepository.GetCustomerOrders(request.CustomerName);

            return _objectMapper.Map<IEnumerable<OrderReadModelDto>,IEnumerable<OrderDto>>(orders);
        }
    }
}
