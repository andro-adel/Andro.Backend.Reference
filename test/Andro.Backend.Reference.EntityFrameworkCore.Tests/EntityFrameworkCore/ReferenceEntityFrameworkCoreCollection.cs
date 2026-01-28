using Xunit;

namespace Andro.Backend.Reference.EntityFrameworkCore;

[CollectionDefinition(ReferenceTestConsts.CollectionDefinitionName)]
public class ReferenceEntityFrameworkCoreCollection : ICollectionFixture<ReferenceEntityFrameworkCoreFixture>
{

}
