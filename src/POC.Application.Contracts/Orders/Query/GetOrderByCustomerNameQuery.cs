using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace POC.Orders.Query
{
    public record GetOrderByCustomerNameQuery : IRequest<IEnumerable<OrderDto>>
    {
        [JsonConstructor]
        public GetOrderByCustomerNameQuery(string customerName)
        {
            CustomerName = customerName;
        }
        public string CustomerName { get; }
    }
}
