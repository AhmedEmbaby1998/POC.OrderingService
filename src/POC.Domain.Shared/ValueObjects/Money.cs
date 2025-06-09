using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace POC.Shared.ValueObjects
{
    public class Money : ValueObject
    {
        public static readonly Money Zero = new(0, "USD");
        public decimal Amount { get;private set; }
        public string Currency { get;private set; }

        [JsonConstructor]
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

        private static decimal CompareTo(Money a, Money b)
        {
            if (a.Currency != b.Currency)
            {
                throw new InvalidOperationException("Cannot compare money with different currencies");
            }
            return a.Amount - b.Amount;
        }
        public static bool operator >(Money a, Money b)
        {
            return CompareTo(a, b) > 0;
        }

        public static bool operator >=(Money a, Money b)
        {
            return CompareTo(a, b) >= 0;
        }
        public static bool operator <(Money a, Money b)
        {
            return CompareTo(a, b) < 0;
        }

        public static bool operator <=(Money a, Money b)
        {
            return CompareTo(a, b) <= 0;
        }

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Money other)
            {
                return CompareTo(this, other) == 0;
            }

            return false;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
