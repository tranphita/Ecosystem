using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(SmartBoxApplicationContractsModule))]
[DependsOn(typeof(AbpHttpClientModule))]
public class SmartBoxHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(SmartBoxApplicationContractsModule).Assembly,
            SmartBoxRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SmartBoxHttpApiClientModule>();
        });
    }
}
