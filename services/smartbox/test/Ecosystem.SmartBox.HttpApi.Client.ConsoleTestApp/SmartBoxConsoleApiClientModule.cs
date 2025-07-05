using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(SmartBoxHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class SmartBoxConsoleApiClientModule : AbpModule { }
