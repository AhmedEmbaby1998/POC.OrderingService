using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities.Events;

namespace POC.Abstractions
{
    public abstract class EventSourcingAggregateRoot<TId> : AuditedAggregateRoot<TId>
    {
        private readonly List<EventSourcedEvent> _uncommittedEvents = [];
        public IReadOnlyCollection<EventSourcedEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        public void RaiseEventSourcedEvent(EventSourcedEvent @event)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event), "Event cannot be null");
            }
            _uncommittedEvents.Add(@event);
        }
        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }


    }
}
