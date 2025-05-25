using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace POC.Shared.ValueObjects
{
    public class Money : ValueObject
    {
        public static readonly Money Zero = new(0, "USD");
        public decimal Amount { get;private set; }
        public string Currency { get;private set; }
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money operator + (Money a, Money b)
        {
            if (!a.Currency.Equals(b.Currency,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("Cannot add money with different currencies");
            }

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
            {
                throw new InvalidOperationException("Cannot subtract money with different currencies");
            }

            return new Money(a.Amount - b.Amount, a.Currency);
        }
        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
