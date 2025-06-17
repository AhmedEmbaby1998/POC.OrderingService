using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using POC.Abstractions;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Json;
using Volo.Abp.Tracing;
using Volo.Abp.Users;

namespace POC.Repositories
{
    public abstract class WriteRepository<TAggregate, TId>
        where TAggregate : EventSourcingAggregateRoot<TId>
    {
        protected readonly IEventStore EventStore;
        protected readonly ILocalEventBus EventBus;
        protected readonly IJsonSerializer JsonSerializer;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICurrentUser _currentUser;
        protected WriteRepository(IEventStore eventStore, ILocalEventBus eventBus, IJsonSerializer jsonSerializer, ICurrentUser currentUser, IHttpContextAccessor contextAccessor)
        {
            EventStore = eventStore;
            EventBus = eventBus;
            JsonSerializer = jsonSerializer;
            _currentUser = currentUser;
            _contextAccessor = contextAccessor;
        }

        public abstract Task<TAggregate> GetAsync(TId id);

        public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken)
        {
            var events = aggregate.UncommittedEvents.Select(e => new StoredEvent
            (
                eventType: e.GetType().AssemblyQualifiedName!,
                eventData: JsonSerializer.Serialize(e),
                createdAt: e.OccurredOn,
                aggregateId: e.AggregateId.ToString(),
                correlationId: _contextAccessor.HttpContext.Request.Headers["X-Correlation-Id"].ToString() ?? string.Empty,
                userId: _currentUser.IsAuthenticated ? _currentUser.GetId().ToString() : string.Empty
            ));

            await EventStore.SaveEventAsync(events, cancellationToken);
            aggregate.ClearUncommittedEvents();

            foreach (var e in aggregate.GetLocalEvents())
            {
                await EventBus.PublishAsync((dynamic)e.EventData);
            }
            aggregate.ClearLocalEvents();
        }
    }
}
