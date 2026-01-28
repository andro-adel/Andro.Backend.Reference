using Volo.Abp.Modularity;

namespace Andro.Backend.Reference;

/* Inherit from this class for your domain layer tests. */
public abstract class ReferenceDomainTestBase<TStartupModule> : ReferenceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
