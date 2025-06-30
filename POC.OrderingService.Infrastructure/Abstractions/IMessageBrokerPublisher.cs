using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.OrderingService.Infrastructure.Abstractions
{
    internal interface IMessageBrokerPublisher
    {
        /// <summary>
        /// Publishes an event to the message broker.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        /// <returns>A task representing the asynchronous operation of completing publishing operaiton.</returns>
        Task PublishAsync<T>(T eventData) where T : class;
    }
}
