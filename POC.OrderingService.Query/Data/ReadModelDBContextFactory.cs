using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace POC.OrderingService.Query.Data
{
    public class ReadModelDBContextFactory : IDesignTimeDbContextFactory<ReadModelDBContext>
    {
        public ReadModelDBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ReadModelDBContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("Read") ??
                throw new InvalidOperationException("Missing 'Read' connection string.")
            );

            return new ReadModelDBContext(optionsBuilder.Options);
        }
    }
}
