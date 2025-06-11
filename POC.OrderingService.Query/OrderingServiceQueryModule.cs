using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.OrderingService.Query.Data;
using POC.OrderingService.Query.Repositories;
using Serilog;
using Volo.Abp.Modularity;

namespace POC.OrderingService.Query
{
    public class OrderingServiceQueryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Information("Configuring OrderingServiceQueryModule...");
            var connectionString = context.Configuration.GetConnectionString("Read");
            Log.Information("Using connection string: {ConnectionString}", connectionString);
            context.Services.AddDbContext<ReadModelDBContext>(options =>
                           options.UseSqlServer(connectionString));
            Log.Information("ReadModelDBContext configured with SQL Server.");
            context.Services.AddScoped<IDbConnection>(provider =>
            {
                return new ResilientDbConnection(new SqlConnection(connectionString));
            });
            Log.Information("ResilientDbConnection registered with SQL Server connection.");
            // Register repositories
            context.Services.AddScoped<IOrderReadModelRepository, OrderReadModelRepository>();
            Log.Information("OrderReadModelRepository registered.");
        }

    }
      
}
