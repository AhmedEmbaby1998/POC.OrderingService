using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace POC
{
    public class StoredEvent :Entity<Guid>
    {
        public StoredEvent(string eventType, string eventData, DateTimeOffset createdAt, string aggregateId)
        {
            EventType = eventType;
            EventData = eventData;
            CreatedAt = createdAt;
            AggregateId = aggregateId;
        }

        public string EventType { get;private set; }
        public string EventData { get;private set; }
        public DateTimeOffset CreatedAt { get;private set; }
        public string AggregateId { get;private set; }
    }
}
