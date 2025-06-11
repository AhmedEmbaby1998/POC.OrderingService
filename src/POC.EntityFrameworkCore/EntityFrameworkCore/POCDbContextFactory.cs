using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace POC.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class POCDbContextFactory : IDesignTimeDbContextFactory<POCDbContext>
{
    public POCDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        POCEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<POCDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Write"));

        return new POCDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../POC.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}