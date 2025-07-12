using Microsoft.Extensions.DependencyInjection;
using Ecosystem.IdentityService;
using Ecosystem.SmartBox.BackgroundServices;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(SmartBoxDomainModule))]
[DependsOn(typeof(SmartBoxApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
[DependsOn(typeof(AbpEventBusModule))]
[DependsOn(typeof(AbpIdentityApplicationModule))]
[DependsOn(typeof(IdentityServiceHttpApiClientModule))]
public class SmartBoxApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<SmartBoxApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SmartBoxApplicationModule>(true);
        });

        // Register Background Services
        context.Services.AddHostedService<UserSyncRetryBackgroundService>();
    }
}
