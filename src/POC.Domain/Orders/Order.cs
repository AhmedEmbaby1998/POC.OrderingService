using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using POC.Orders.Events;
using POC.Orders.Exceptions;
using POC.Shared;
using POC.Shared.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;

namespace POC.Orders
{
    public class Order : EventSourcedAggregateRoot<OrderId>
    {
        private Order()
        {
        }

        private Order(OrderId id,string customerName, Address address)
        {
            var e = new OrderCreatedEvent(id, customerName, DateTime.Now);
            this.AddEvent(e);
            Apply(e);
        }

        public static Order Create(OrderId id, string customerName, Address address)
        {
            return new Order(id, customerName, address);
        }

        public string CustomerName { get;private set; }
        public Address Address { get; private set; }
        public DateOnly? DeliveryDate {private set; get; }
        public Money TotalPrice { get;private set; }

        private readonly HashSet<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => [.. _items];


        public void SetItems(IEnumerable<OrderItem> items)
        {
            var e = new OrdeSetItemsEvent(this.Id,items);
            this.AddEvent(e);
            Apply(e);
        }
        public void Apply(OrdeSetItemsEvent @event)
        {
            if (DeliveryDate is { })
            {
                throw new CanNotModifyDeliveredOrder(this.Id);
            }

            this._items.Clear();
            this.TotalPrice = Money.Zero;

            foreach (var item in @event.Items)
            {
                this._items.Add(item);
                this.TotalPrice += item.Price;
            }
        }

        public void Apply(OrderCreatedEvent orderCreatedEvent)
        {
            this.Id = orderCreatedEvent.OrderId;
            this.CustomerName = orderCreatedEvent.CustomerName;
            this.DeliveryDate = null;
            this.TotalPrice = Money.Zero;
        }

        public static Order ApplyEvent(IEnumerable<StoredEvent> @event)
        {
            var order = new Order();
            foreach (var e in @event)
            {
                var eventType = Type.GetType(e.EventType);
                var domainEvent = JsonSerializer.Deserialize(e.EventData, eventType);

                ((dynamic)order).Apply((dynamic)domainEvent);
            }
            return order;
        }


    }
}
