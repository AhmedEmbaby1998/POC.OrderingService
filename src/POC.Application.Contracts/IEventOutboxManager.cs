using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace POC
{
    public interface IEventOutboxManager
    {
        Task EnqueueAsync(OutgoingEventInfo outgoingEventInfo);
    }
}
