using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Andro.Backend.Reference.Data;

/* This is used if database provider does't define
 * IReferenceDbSchemaMigrator implementation.
 */
public class NullReferenceDbSchemaMigrator : IReferenceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
