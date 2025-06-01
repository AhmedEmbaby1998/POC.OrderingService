using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities.Events;

namespace POC.Shared
{
    public abstract class EventSourcedAggregateRoot<TId> : FullAuditedAggregateRoot<TId>
    {
        private readonly List<object> _uncommittedEvents = new();
        public IReadOnlyList<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        public void AddEvent(object @event)
        {
            AddLocalEvent(@event);
            _uncommittedEvents.Add(@event);
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

    }
}
