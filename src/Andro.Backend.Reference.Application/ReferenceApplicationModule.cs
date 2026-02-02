using System.Threading.Tasks;
using Andro.Backend.Reference.Products;
using Andro.Backend.Reference.Products.Workers;
using Volo.Abp;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;

namespace Andro.Backend.Reference;

[DependsOn(
    typeof(ReferenceDomainModule),
    typeof(ReferenceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpBackgroundJobsModule),
    typeof(AbpBackgroundWorkersModule)
    )]
public class ReferenceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure background jobs
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true;
        });
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // Register background workers
        await context.AddBackgroundWorkerAsync<StockCheckWorker>();
    }
}
