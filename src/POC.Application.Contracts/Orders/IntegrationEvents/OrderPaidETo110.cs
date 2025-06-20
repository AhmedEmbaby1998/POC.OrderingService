using System;
using POC.Shared.ValueObjects;
using Volo.Abp.EventBus;

namespace POC.Orders.IntegrationEvents
{
    [EventName("OrderPaid")]
    public record OrderPaidETo110(Guid OrderId, Money TotalPrice);
}
