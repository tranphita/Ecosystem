using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Ecosystem.Projects.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(ProjectsHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class ProjectsConsoleApiClientModule : AbpModule { }
