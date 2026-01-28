using Andro.Backend.Reference.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Andro.Backend.Reference.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ReferenceController : AbpControllerBase
{
    protected ReferenceController()
    {
        LocalizationResource = typeof(ReferenceResource);
    }
}
