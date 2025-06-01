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
using Polly;

namespace POC.OrderingService.Query
{
    public static class IOC
    {
        public static void RegisterQueryServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<ReadModelDBContext>(options =>
               options.UseNpgsql(configuration.GetConnectionString("Read")));

            services.AddScoped<IDbConnection>(provider =>
            {
                var connectionString =configuration.GetConnectionString("DefaultConnection");

                return new ResilientDbConnection(new SqlConnection(connectionString));
            });

            // Register repositories
            services.AddScoped<IOrderReadModelRepository, OrderReadModelRepository>();
        }
    }
}
