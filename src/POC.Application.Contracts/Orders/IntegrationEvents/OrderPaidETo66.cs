using System;
using POC.Shared.ValueObjects;
using Volo.Abp.EventBus;

namespace POC.Orders.IntegrationEvents
{
    [EventName("OrderPaid")]
    public record OrderPaidETo66(Guid OrderId, Money TotalPrice);
}
