using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using POC.Abstractions;
using POC.Orders.Events.EventsSourced;
using POC.Orders.Exceptions;
using POC.Orders.Factories;
using POC.Shared.ValueObjects;

namespace POC.Orders
{
    public class Order : EventSourcingAggregateRoot<OrderId>
    {
        private Order()
        {
        }

        private Order(OrderId id,string customerName, Address address)
        {
            var e = new OrderCreatedEventSourced(id, customerName,address,DateTime.Now);
            this.RaiseEventSourcedEvent(e);
            Apply(e);
            this.AddLocalEvent(this.ToOrderCreatedEvent());
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
            var e = new OrdeSetItemsEventSourced(this.Id,items);
            RaiseEventSourcedEvent(e);
            Apply(e);
            AddLocalEvent(e);
        }

        public void Pay(Money amount)
        {
            var e = new OrderPaidEventSourced(this.Id,CustomerName,amount);
            this.RaiseEventSourcedEvent(e);
            Apply(e);
            this.AddLocalEvent(this.ToOrderPaidEvent());
        }

        private void Apply(OrderPaidEventSourced e)
        {
            if (DeliveryDate is { })
            {
                throw new CanNotModifyDeliveredOrder(this.Id);
            }
            if (e.TotalPrice < this.TotalPrice)
            {
                throw new NotEnoughMoneyToPayOrder(this.Id, e.TotalPrice, this.TotalPrice);
            }
            this.DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
        }

        private void Apply(OrdeSetItemsEventSourced @event)
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

        private void Apply(OrderCreatedEventSourced orderCreatedEvent)
        {
            this.Id = orderCreatedEvent.OrderId;
            this.CustomerName = orderCreatedEvent.CustomerName;
            this.DeliveryDate = null;
            this.Address = orderCreatedEvent.Address;
            this.TotalPrice = Money.Zero;
        }

        public static Order Rehydrate(IEnumerable<StoredEvent> @event)
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
