using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;

namespace POC.OrderingService.Infrastructure.OutOfBox
{
    internal class EventOutOfBoxManager : IEventOutboxManager
    {
        private readonly IRepository<OutgoingEventRecord, Guid> _repository;

        public EventOutOfBoxManager(IRepository<OutgoingEventRecord, Guid> repository)
        {
            _repository = repository;
        }

        public async Task EnqueueAsync(OutgoingEventInfo outgoingEventInfo)
        {
            await _repository.InsertAsync(
                new OutgoingEventRecord(outgoingEventInfo)
            );
        }
    }
}
