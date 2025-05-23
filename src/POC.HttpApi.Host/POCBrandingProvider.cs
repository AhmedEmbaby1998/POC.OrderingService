using Microsoft.Extensions.Localization;
using POC.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace POC;

[Dependency(ReplaceServices = true)]
public class POCBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<POCResource> _localizer;

    public POCBrandingProvider(IStringLocalizer<POCResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
