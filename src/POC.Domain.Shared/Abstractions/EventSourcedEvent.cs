using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.Abstractions
{
    public abstract record EventSourcedEvent
    {
        public Guid AggregateId {get;protected set; }
        public DateTimeOffset OccurredOn { get;protected set; }



        protected EventSourcedEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            OccurredOn = DateTimeOffset.UtcNow;
        }

    }
}
