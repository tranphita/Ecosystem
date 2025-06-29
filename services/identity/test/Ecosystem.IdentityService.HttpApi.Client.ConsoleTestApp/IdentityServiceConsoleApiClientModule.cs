using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Ecosystem.IdentityService.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(IdentityServiceHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class IdentityServiceConsoleApiClientModule : AbpModule { }
