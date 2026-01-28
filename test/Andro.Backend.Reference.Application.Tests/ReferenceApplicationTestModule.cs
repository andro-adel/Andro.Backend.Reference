using Volo.Abp.Modularity;

namespace Andro.Backend.Reference;

[DependsOn(
    typeof(ReferenceApplicationModule),
    typeof(ReferenceDomainTestModule)
)]
public class ReferenceApplicationTestModule : AbpModule
{

}
