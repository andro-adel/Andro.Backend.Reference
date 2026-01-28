using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Andro.Backend.Reference.Pages;

[Collection(ReferenceTestConsts.CollectionDefinitionName)]
public class Index_Tests : ReferenceWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
