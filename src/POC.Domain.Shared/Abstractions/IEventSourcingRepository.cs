﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Volo.Abp.Domain.Entities;

namespace POC.Abstractions
{
    public interface IEventSourcingRepository<TAggregate, TId> where TAggregate : IAggregateRoot<TId>
    {
        public Task<TAggregate> GetAsync(TId id);
        public Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken);
    }
}
