using Microsoft.AspNetCore.Builder;
using Andro.Backend.Reference;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Andro.Backend.Reference.Web.csproj"); 
await builder.RunAbpModuleAsync<ReferenceWebTestModule>(applicationName: "Andro.Backend.Reference.Web");

public partial class Program
{
}
