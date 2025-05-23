using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace POC.Orders.Exceptions
{
    internal class CanNotModifyDeliveredOrder: BusinessException
    {
        public CanNotModifyDeliveredOrder(Guid orderId) : base($"Order {orderId} is already delivered and cannot be modified.")
        {
        }
    }

}
