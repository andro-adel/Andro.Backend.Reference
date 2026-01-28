using Andro.Backend.Reference.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Andro.Backend.Reference.Web.Pages;

public abstract class ReferencePageModel : AbpPageModel
{
    protected ReferencePageModel()
    {
        LocalizationResourceType = typeof(ReferenceResource);
    }
}
