using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;
using POC.Orders.Events.DomainEvents;
using POC.Orders.Events.EventsSourced;
using POC.Orders.IntegrationEvents;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace POC.Features.Orders.EventsHandlers
{
    public class OrderReadModelSyncHandler : ILocalEventHandler<OrderEvent>,IScopedDependency,
        INotificationHandler<OrderEvent>
    {
        private readonly IOrderReadModelRepository _orderReadModelRepository;

        public OrderReadModelSyncHandler(IOrderReadModelRepository orderReadModelRepository)
        {
            _orderReadModelRepository = orderReadModelRepository ?? throw new ArgumentNullException(nameof(orderReadModelRepository));
        }

        public Task Handle(OrderEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleEventAsync(OrderEvent eventData)
        {
            var orderReadModel = new OrderReadModel
            {
                Id = eventData.Id,
                CustomerName = eventData.CustomerName,
                DeliveryDate = eventData.DeliveryDate,
                Amount = eventData.TotalPrice.Amount,
                Currency = eventData.TotalPrice.Currency,
                City = eventData.Address.City,
                Street = eventData.Address.Street,
                State = eventData.Address.State,
                ZipCode = eventData.Address.ZipCode,
                Items = eventData.Items.Select(item => new OrderItemReadModel
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Count = item.Quantity.Count,
                    Unit = item.Quantity.Unit,
                    Amount = item.Price.Amount,
                    Currency = item.Price.Currency
                }).ToList()
            };

            if (eventData.EventType is OrderEventType.Created)
            {
                await _orderReadModelRepository.InsertAsync(orderReadModel);
            }
        }
    }
}
