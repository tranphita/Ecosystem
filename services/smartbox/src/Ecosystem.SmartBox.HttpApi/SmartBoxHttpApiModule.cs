using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Ecosystem.SmartBox.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(SmartBoxApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreMvcModule))]
public class SmartBoxHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SmartBoxHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Get<SmartBoxResource>().AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
