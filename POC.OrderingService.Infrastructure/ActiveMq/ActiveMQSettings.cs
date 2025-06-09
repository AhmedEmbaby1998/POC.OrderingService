using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.OrderingService.Infrastructure.ActiveMq
{
    internal class ActiveMQSettings
    {
        internal string BrokerUri { get; init; }
        internal string UserName { get; init; }
        internal string Password { get; init; }
        internal string QueueName { get; init; }
    }
}