﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Values;

namespace POC.Orders
{
    public class OrderItem :AuditedEntity<Guid>
    {
        private OrderItem()
        {
        }
        [JsonConstructor]
        public OrderItem(string productName, Quantity quantity, Money price)
        {
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }

        public OrderItem(string productName, Quantity quantity)
        {
            ProductName = productName;
            Quantity = quantity;
        }

        public Order Order {private set;get; }
        public OrderId OrderId { get;private set; }
        public string ProductName { get;private set; }
        public Quantity Quantity { get;private set; }
        public Money Price { get; private set; }


    }

}
