using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Values;

namespace POC.Orders
{
    public class OrderItem :ValueObject
    {
        private OrderItem()
        {
        }
        public OrderItem(string productName, Quantity quantity, Money price)
        {
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }

        public Order Order {private set;get; }
        public OrderId OrderId { get;private set; }
        public string ProductName { get;private set; }
        public Quantity Quantity { get;private set; }
        public Money Price { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ProductName;
            yield return Quantity;
            yield return Price;
            yield return OrderId;
        }
    }

}
