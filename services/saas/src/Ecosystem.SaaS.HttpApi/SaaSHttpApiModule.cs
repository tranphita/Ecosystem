using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Ecosystem.SaaS.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Ecosystem.SaaS;

[DependsOn(typeof(SaaSApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreMvcModule))]
[DependsOn(typeof(AbpTenantManagementHttpApiModule))]
public class SaaSHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SaaSHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Get<SaaSResource>().AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
