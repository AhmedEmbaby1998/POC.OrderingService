using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.Invoices
{
    public sealed record InvoiceId
    {
        public Guid Id { get; private set; }
        private InvoiceId(Guid id)
        {
            Id = id;
        }
        public static implicit operator Guid(InvoiceId orderId) => orderId.Id;

        public static InvoiceId New(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("InvoiceId cannot be empty", nameof(id));
            }
            return new InvoiceId(id);
        }
    }
}
