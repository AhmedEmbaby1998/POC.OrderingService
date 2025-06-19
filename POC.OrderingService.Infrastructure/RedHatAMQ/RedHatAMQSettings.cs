using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.OrderingService.Infrastructure.ActiveMq
{
    public class RedHatAMQSettings
    {
        public string BrokerUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}