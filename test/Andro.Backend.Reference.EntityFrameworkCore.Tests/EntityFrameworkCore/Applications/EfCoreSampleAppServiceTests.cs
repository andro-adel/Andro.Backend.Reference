using Andro.Backend.Reference.Samples;
using Xunit;

namespace Andro.Backend.Reference.EntityFrameworkCore.Applications;

[Collection(ReferenceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ReferenceEntityFrameworkCoreTestModule>
{

}
