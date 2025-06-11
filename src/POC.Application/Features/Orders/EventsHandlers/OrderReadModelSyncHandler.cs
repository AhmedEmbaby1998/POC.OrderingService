using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;
using POC.Orders.Events.EventsSourced;
using POC.Orders.IntegrationEvents;
using Volo.Abp.EventBus;

namespace POC.Features.Orders.EventsHandlers
{
    internal class OrderReadModelSyncHandler : ILocalEventHandler<OrderCreatedEvent>
    {
        private readonly IOrderReadModelRepository _orderReadModelRepository;

        internal OrderReadModelSyncHandler(IOrderReadModelRepository orderReadModelRepository)
        {
            _orderReadModelRepository = orderReadModelRepository ?? throw new ArgumentNullException(nameof(orderReadModelRepository));
        }
        public async Task HandleEventAsync(OrderCreatedEvent eventData)
        {
            var orderReadModel = new OrderReadModel
            {
                Id = eventData.OrderId,
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
                    Id = item.OrderId,
                    ProductName = item.ProductName,
                    Count = item.Quantity.Count,
                    Unit = item.Quantity.Unit,
                    Amount = item.Price.Amount,
                    Currency = item.Price.Currency
                }).ToList()
            };
            await _orderReadModelRepository.InsertAsync(orderReadModel);
        }
    }
}
