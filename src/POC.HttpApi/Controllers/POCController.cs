using POC.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace POC.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class POCController : AbpControllerBase
{
    protected POCController()
    {
        LocalizationResource = typeof(POCResource);
    }
}
