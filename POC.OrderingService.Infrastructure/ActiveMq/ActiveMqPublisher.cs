using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace POC.OrderingService.Infrastructure.ActiveMq
{
    internal class ActiveMqPublisher : IPublisher,IDistributedEventBus
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

        public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler, new()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedException();
        }
    }
}
