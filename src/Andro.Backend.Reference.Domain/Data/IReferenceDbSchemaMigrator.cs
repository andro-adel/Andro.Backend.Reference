using System.Threading.Tasks;

namespace Andro.Backend.Reference.Data;

public interface IReferenceDbSchemaMigrator
{
    Task MigrateAsync();
}
