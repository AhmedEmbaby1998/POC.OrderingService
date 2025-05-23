using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POC.Orders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace POC.EntitiesConfiguration.Orders
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ConfigureByConvention(); //auto configure for the base class props
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);
            builder.Property(a=>a.Id)
                .HasConversion(
                    convertToProviderExpression: id => id.Id,
                    convertFromProviderExpression: id => OrderId.New(id)
                );
            builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(100);
            builder.OwnsOne(x => x.Address, a =>
            {
                a.WithOwner();
                a.Property(x => x.Street).HasColumnName("Street").HasMaxLength(200);
                a.Property(x => x.City).HasColumnName("City").HasMaxLength(100);
                a.Property(x => x.State).HasColumnName("State").HasMaxLength(50);
                a.Property(x => x.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            });
            builder.OwnsMany(x => x.Items, i =>
            {
                i.WithOwner().HasForeignKey("OrderId");
                i.HasKey("Id");
                i.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
                i.OwnsOne(a => a.Price, p =>
                {
                    p.WithOwner();
                    p.Property(x => x.Amount).HasColumnName("Amount").IsRequired();
                    p.Property(x => x.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
                });
                i.OwnsOne(a=>a.Quantity, q =>
                {
                    q.WithOwner();
                    q.Property(x => x.Count).HasColumnName("Count").IsRequired();
                    q.Property(x => x.Unit).HasColumnName("Unit").IsRequired().HasMaxLength(50);
                });
            });
            builder.OwnsOne(x => x.TotalPrice, p =>
            {
                p.WithOwner();
                p.Property(x => x.Amount).HasColumnName("TotalPrice_Amount").IsRequired();
                p.Property(x => x.Currency).HasColumnName("TotalPrice_Currency").IsRequired().HasMaxLength(3);
            });
        }
    }
}
