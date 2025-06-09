using System;
using POC.Shared.ValueObjects;
using Volo.Abp;

namespace POC.Orders.Exceptions
{
    [Serializable]
    internal class NotEnoughMoneyToPayOrder : BusinessException
    {
        public NotEnoughMoneyToPayOrder(OrderId id, Money amount, Money totalPrice)
            : base("Ordering:NotEnoughMoneyToPayOrder",
                $"Not enough money to pay order {id}. Amount: {amount}, Total Price: {totalPrice}")
        {
        }
    }
}