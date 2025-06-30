using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using POC.OrderingService.Infrastructure.Abstractions;
using POC.OrderingService.Infrastructure.OutOfBox;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace POC.OrderingService.Infrastructure.RedHatAMQ
{
    internal class RedhatAMQPublisher : IMessageBrokerPublisher
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DBOutOfBoxEventBus> _logger;
        private readonly IPublishEndpoint _publisher;

        public RedhatAMQPublisher(
            ILogger<DBOutOfBoxEventBus> logger,
            IHttpContextAccessor httpContextAccessor,
            IPublishEndpoint publisher)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _publisher = publisher;
        }


        public async Task PublishAsync<T>(T eventData) where T : class =>
            await _publisher.Publish(eventData, ctx =>
            {
                ctx.Durable = true;
                ctx.CorrelationId = GetCorrelationId();
            });

        private Guid? GetCorrelationId()
        {
            var correlation = _httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString();
            return Guid.TryParse(correlation, out var guid) ? guid : null;
        }
    }
}
