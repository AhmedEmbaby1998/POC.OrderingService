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
using Volo.Abp.Modularity;

namespace POC.OrderingService.Query
{
    public class OrderingServiceQueryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDbContext<ReadModelDBContext>(options =>
                           options.UseNpgsql(context.Configuration.GetConnectionString("Read")));

            context.Services.AddScoped<IDbConnection>(provider =>
            {
                var connectionString = context.Configuration.GetConnectionString("Read");

                return new ResilientDbConnection(new SqlConnection(connectionString));
            });

            // Register repositories
            context.Services.AddScoped<IOrderReadModelRepository, OrderReadModelRepository>();
        }

    }
      
}
