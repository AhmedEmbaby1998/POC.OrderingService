using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POC.Invoices;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace POC.EntitiesConfiguration.Invoices
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ConfigureByConvention(); //auto configure for the base class props
            builder.ToTable("Invoices");
            builder.HasKey(x => x.Id);
            builder.Property(a => a.Id)
                .HasConversion(
                    convertToProviderExpression: id => id.Id,
                    convertFromProviderExpression: id => InvoiceId.New(id)
                );
            builder.Property(x=>x.CustomerName).HasMaxLength(100);
            builder.OwnsOne(a => a.TotalPrice, p =>
            {
                p.WithOwner();
                p.Property(x => x.Amount).HasColumnName("Amount").IsRequired();
                p.Property(x => x.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
            });
        }
    }
}
