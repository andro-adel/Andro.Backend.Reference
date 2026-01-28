using Andro.Backend.Reference.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Andro.Backend.Reference.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ReferenceEntityFrameworkCoreModule),
    typeof(ReferenceApplicationContractsModule)
)]
public class ReferenceDbMigratorModule : AbpModule
{
}
