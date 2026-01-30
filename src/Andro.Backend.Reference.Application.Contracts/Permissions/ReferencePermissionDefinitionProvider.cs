using Andro.Backend.Reference.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Andro.Backend.Reference.Permissions;

public class ReferencePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ReferencePermissions.GroupName, L("Permission:Reference"));

        var productsPermission = myGroup.AddPermission(
            ReferencePermissions.Products.Default,
            L("Permission:Products")
        );

        productsPermission.AddChild(
            ReferencePermissions.Products.Create,
            L("Permission:Products.Create")
        );

        productsPermission.AddChild(
            ReferencePermissions.Products.Edit,
            L("Permission:Products.Edit")
        );

        productsPermission.AddChild(
            ReferencePermissions.Products.Delete,
            L("Permission:Products.Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ReferenceResource>(name);
    }
}
