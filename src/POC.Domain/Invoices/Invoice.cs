using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.Shared.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;

namespace POC.Invoices
{
    public class Invoice: AuditedAggregateRoot<InvoiceId>
    {
        public Guid OrderId { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public string CustomerName { get; private set; }
        public Money TotalPrice { get; set; }
    }
}
