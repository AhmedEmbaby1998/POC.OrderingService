using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace POC.Orders.Query
{
    public record GetOrderByIdQuery : IRequest<OrderDto>
    {
        [JsonConstructor]
        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
        public Guid OrderId { get; }
    }
}
