using Volo.Abp.Modularity;

namespace Andro.Backend.Reference;

[DependsOn(
    typeof(ReferenceDomainModule),
    typeof(ReferenceTestBaseModule)
)]
public class ReferenceDomainTestModule : AbpModule
{

}
