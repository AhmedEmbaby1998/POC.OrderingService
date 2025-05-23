using POC.Localization;
using Volo.Abp.Application.Services;

namespace POC;

/* Inherit your application services from this class.
 */
public abstract class POCAppService : ApplicationService
{
    protected POCAppService()
    {
        LocalizationResource = typeof(POCResource);
    }
}
