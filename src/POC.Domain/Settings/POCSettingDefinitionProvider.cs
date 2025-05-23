using Volo.Abp.Settings;

namespace POC.Settings;

public class POCSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(POCSettings.MySetting1));
    }
}
