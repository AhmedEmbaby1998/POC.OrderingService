using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using POC.OrderingService.Infrastructure.Abstractions;
using Serilog;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace POC.OrderingService.Infrastructure.OutOfBox
{
    [Queue("outofbox")]
    internal class HangFireOutOfBoxEventBus : AsyncBackgroundJob<object>,IDistributedEventBus
    {
        private readonly IMessageBrokerPublisher _publisher;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HangFireOutOfBoxEventBus(IMessageBrokerPublisher publisher, IJsonSerializer jsonSerializer, IUnitOfWorkManager unitOfWorkManager, IHttpContextAccessor httpContextAccessor)
        {
            _publisher = publisher;
            _jsonSerializer = jsonSerializer;
            _unitOfWorkManager = unitOfWorkManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where T : class
        {
            try
            {
                if (useOutbox)
                {
                    BackgroundJob.Enqueue<HangFireOutOfBoxEventBus>(job => job.ExecuteAsync(eventData));
                }

                if (onUnitOfWorkComplete && _unitOfWorkManager.Current is { } uow)
                {
                    uow.OnCompleted(async () => await _publisher.PublishAsync(eventData));
                }

                await _publisher.PublishAsync(eventData);
            }catch(Exception e)
            {

            }
        }

        /// <summary>
        /// Hangfire Worker Method
        /// </summary>
        /// <param name="eventData">event to be published</param>
        /// <returns></returns>
        public override async Task ExecuteAsync(object eventData)
        {
            Log.Logger.Information("Hangfire Start");
            var x = eventData.GetType().GetFullNameWithAssemblyName();
            var @event = new OutgoingEventInfo(
                id: Guid.NewGuid(),
                eventName: eventData.GetType().GetFullNameWithAssemblyName(),
                eventData: _jsonSerializer.Serialize(eventData).GetBytes(),
                creationTime: DateTime.UtcNow);

            @event.SetCorrelationId(GetCorrelationId()?.ToString() ?? string.Empty);

            await _publisher.PublishAsync(@event);

            Logger.LogInformation("publish the message from hangire {Message}", _jsonSerializer.Serialize(@event));
        }

        public Task PublishAsync<T>(T eventData, bool onUnitOfWorkComplete = true) where T : class =>
            PublishAsync(eventData, onUnitOfWorkComplete, useOutbox: true);

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
        {
            var method = typeof(DBOutOfBoxEventBus)
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == nameof(PublishAsync) &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 3) ?? throw new InvalidOperationException($"Could not find method PublishAsync<T>");

            var genericMethod = method.MakeGenericMethod(eventType);
            return (Task)genericMethod.Invoke(this, [eventData, onUnitOfWorkComplete, useOutbox])!;
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true) =>
            PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox: true);



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



        #region Private Helpers
 

        private Guid? GetCorrelationId()
        {
            var correlation = _httpContextAccessor.HttpContext?.Request?.Headers["X-Correlation-Id"].ToString();
            return Guid.TryParse(correlation, out var guid) ? guid : null;
        }
        #endregion Private Helpers
    }
}
