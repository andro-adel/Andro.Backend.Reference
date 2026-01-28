using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Andro.Backend.Reference.Data;
using Volo.Abp.DependencyInjection;

namespace Andro.Backend.Reference.EntityFrameworkCore;

public class EntityFrameworkCoreReferenceDbSchemaMigrator
    : IReferenceDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreReferenceDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ReferenceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ReferenceDbContext>()
            .Database
            .MigrateAsync();
    }
}
