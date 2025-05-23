using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.Orders
{
    public sealed record OrderId
    {
        public Guid Id { get; private set; }
        private OrderId(Guid id)
        {
            Id = id;
        }
        public static implicit operator Guid(OrderId orderId) => orderId.Id;

        public static OrderId New(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("OrderId cannot be empty", nameof(id));
            }
            return new OrderId(id);
        }
    }
}
