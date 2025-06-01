using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace POC.OrderingService.Query.Data
{
    internal class ReadModelDBContext : DbContext
    {
        public ReadModelDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ReadModels.Orders.OrderReadModel> Orders { get; set; }
    }
}
