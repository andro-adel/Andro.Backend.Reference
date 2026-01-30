using System.Threading.Tasks;
using Andro.Backend.Reference.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Andro.Backend.Reference.DbMigrator;

public class ProductPermissionsDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionManager _permissionManager;
    private readonly IIdentityRoleRepository _roleRepository;

    public ProductPermissionsDataSeedContributor(
        IPermissionManager permissionManager,
        IIdentityRoleRepository roleRepository)
    {
        _permissionManager = permissionManager;
        _roleRepository = roleRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var adminRole = await _roleRepository.FindByNormalizedNameAsync("ADMIN");
        
        if (adminRole != null)
        {
            await _permissionManager.SetForRoleAsync(
                adminRole.Name, 
                ReferencePermissions.Products.Default, 
                true
            );
            
            await _permissionManager.SetForRoleAsync(
                adminRole.Name, 
                ReferencePermissions.Products.Create, 
                true
            );
            
            await _permissionManager.SetForRoleAsync(
                adminRole.Name, 
                ReferencePermissions.Products.Edit, 
                true
            );
            
            await _permissionManager.SetForRoleAsync(
                adminRole.Name, 
                ReferencePermissions.Products.Delete, 
                true
            );
        }
    }
}
