using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POC.Orders
{
    public sealed record OrderId
    {
        public Guid Value { get; private set; }
        [JsonConstructor]
        private OrderId(Guid value)
        {
            Value = value;
        }
        public static implicit operator string(OrderId orderId) => orderId.ToString();

        public static OrderId New(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("OrderId cannot be empty", nameof(id));
            }
            return new OrderId(id);
        }

        public static OrderId New()
        {
            return New(Guid.NewGuid());
        }
    }
}
