using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Andro.Backend.Reference.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class ReferenceDbContextFactory : IDesignTimeDbContextFactory<ReferenceDbContext>
{
    public ReferenceDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        ReferenceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<ReferenceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new ReferenceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Andro.Backend.Reference.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
