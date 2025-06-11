using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.Orders.Events.DomainEvents
{
    public enum OrderEventType
    {
        Created = 0,
        Paid = 1,
        Cancelled = 2,
    }
}
