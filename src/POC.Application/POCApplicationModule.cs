using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using POC.Features.Orders.Mappers;

namespace POC;

[DependsOn(
    typeof(POCDomainModule),
    typeof(POCApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class POCApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<POCApplicationModule>();
        });

        context.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(POCApplicationModule).Assembly);
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            // Add all mapping profiles in this assembly
            options.AddMaps<POCApplicationModule>();

            options.ValidateProfile<OrderMappingProfile>();
        });
    }
}
