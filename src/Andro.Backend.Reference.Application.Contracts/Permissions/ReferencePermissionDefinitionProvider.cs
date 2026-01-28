using Andro.Backend.Reference.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Andro.Backend.Reference.Permissions;

public class ReferencePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ReferencePermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(ReferencePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ReferenceResource>(name);
    }
}
