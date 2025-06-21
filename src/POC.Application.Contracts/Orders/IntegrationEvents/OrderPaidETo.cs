using System;
using POC.Shared.ValueObjects;
using Volo.Abp.EventBus;

namespace POC.Orders.IntegrationEvents
{
    public record OrderPaidETo(Guid OrderId, Money TotalPrice);
}
