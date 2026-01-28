using Andro.Backend.Reference.Samples;
using Xunit;

namespace Andro.Backend.Reference.EntityFrameworkCore.Domains;

[Collection(ReferenceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ReferenceEntityFrameworkCoreTestModule>
{

}
