using Andro.Backend.Reference.Localization;
using Volo.Abp.Application.Services;

namespace Andro.Backend.Reference;

/* Inherit your application services from this class.
 */
public abstract class ReferenceAppService : ApplicationService
{
    protected ReferenceAppService()
    {
        LocalizationResource = typeof(ReferenceResource);
    }
}
