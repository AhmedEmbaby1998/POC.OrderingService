using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities.Events;

namespace POC.Shared
{
    public abstract class EventSourcingAggregateRoot<TId> : AuditedAggregateRoot<TId>
    {
        private readonly List<object> _uncommittedEvents = [];
        public IReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();


        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }


    }
}
