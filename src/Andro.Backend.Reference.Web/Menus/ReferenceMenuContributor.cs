using System.Threading.Tasks;
using Andro.Backend.Reference.Localization;
using Andro.Backend.Reference.Permissions;
using Andro.Backend.Reference.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

namespace Andro.Backend.Reference.Web.Menus;

public class ReferenceMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ReferenceResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                ReferenceMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );


        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        //Administration->Tenant Management (Multi-tenancy is enabled)
        administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }
}
