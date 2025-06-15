using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POC.OrderingService.Query.Contracts.ReadModels.Orders;
using Serilog;

namespace POC.OrderingService.Query.Data
{
    internal class ReadModelDBContext : DbContext
    {
        public ReadModelDBContext(DbContextOptions<ReadModelDBContext> options) : base(options)
        {
            Log.Logger.Information("ReadModelDBContext initialized with options: {Options}", options);
        }

        public DbSet<OrderReadModel> OrderReadModel { get; set; }
    }
}
