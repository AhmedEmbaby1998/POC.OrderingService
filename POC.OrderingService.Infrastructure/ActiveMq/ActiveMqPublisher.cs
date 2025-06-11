using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace POC.OrderingService.Infrastructure.ActiveMq
{
    internal class ActiveMqPublisher : IPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public ActiveMqPublisher(IPublishEndpoint publishEndpoint)
        {
            this._publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task PublishAsync<T>(T @event) where T : class
        {
            await _publishEndpoint.Publish(@event);
        }
    }
}
