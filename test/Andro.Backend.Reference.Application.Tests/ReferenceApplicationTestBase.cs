using Volo.Abp.Modularity;

namespace Andro.Backend.Reference;

public abstract class ReferenceApplicationTestBase<TStartupModule> : ReferenceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
