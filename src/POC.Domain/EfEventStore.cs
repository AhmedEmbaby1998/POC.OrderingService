using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Json;

namespace POC
{
    internal class EfEventStore : IEventStore
    {
        private readonly IRepository<StoredEvent, Guid> _repository;
        private readonly IJsonSerializer _jsonSerializer;
        public EfEventStore(IRepository<StoredEvent, Guid> repository, IJsonSerializer jsonSerializer)
        {
            _repository = repository;
            _jsonSerializer = jsonSerializer;
        }

        public async Task SaveEventAsync(IEnumerable<StoredEvent> storedEvents,CancellationToken cancellationToken)
        {
            await _repository.InsertManyAsync(storedEvents,false,cancellationToken);
        }
        public async Task<IEnumerable<StoredEvent>> GetEventsAsync(Guid aggregateId)
        {
            return await _repository.GetListAsync(e => e.AggregateId == aggregateId.ToString());
        }
    }
}
