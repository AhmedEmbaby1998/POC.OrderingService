using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POC
{
    public interface IEventStore
    {
        Task<IEnumerable<StoredEvent>> GetEventsAsync(Guid aggregateId);
        Task SaveEventAsync(IEnumerable<StoredEvent> storedEvents,CancellationToken cancellationToken);
    }
}
