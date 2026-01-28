using Volo.Abp.Settings;

namespace Andro.Backend.Reference.Settings;

public class ReferenceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ReferenceSettings.MySetting1));
    }
}
