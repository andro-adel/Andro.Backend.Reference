using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Andro.Backend.Reference.Localization;

namespace Andro.Backend.Reference.Web;

[Dependency(ReplaceServices = true)]
public class ReferenceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ReferenceResource> _localizer;

    public ReferenceBrandingProvider(IStringLocalizer<ReferenceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
