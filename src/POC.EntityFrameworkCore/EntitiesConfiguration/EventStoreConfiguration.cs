using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace POC.EntitiesConfiguration
{
    internal class EventStoreConfiguration : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.ConfigureByConvention(); //auto configure for the base class props
            builder.ToTable("StoredEvents");
            builder.HasKey(x => x.Id);
            builder.Property(a => a.EventType).IsRequired();
            builder.Property(a => a.EventData).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.AggregateId).IsRequired();
        }
    }
}
