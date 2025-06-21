using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using POC.OrderingService.Query.Abstraction.Repositories;
using POC.OrderingService.Query.Data;
using POC.OrderingService.Query.Repositories;
using Serilog;
using Volo.Abp.Modularity;
using static ResilientDbConnection;

namespace POC.OrderingService.Query
{
    public class OrderingServiceQueryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Information("Configuring OrderingServiceQueryModule...");

            Configure<SqlResilienceOptions>(context.Configuration.GetSection("SqlResilience"));

            var connectionString = context.Configuration.GetConnectionString("Read");
            Log.Information("Using connection string: {ConnectionString}", connectionString);
            context.Services.AddDbContext<ReadModelDBContext>(options =>
                           options.UseSqlServer(connectionString));
            Log.Information("ReadModelDBContext configured with SQL Server.");
            context.Services.AddScoped<DbConnection>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<SqlResilienceOptions>>();
                return new ResilientDbConnection(new SqlConnection(connectionString),options);
            });
            Log.Information("ResilientDbConnection registered with SQL Server connection.");
            // Register repositories
            context.Services.AddScoped<IOrderReadModelRepository, OrderReadModelRepository>();
            Log.Information("OrderReadModelRepository registered.");
        }

    }
      
}
