using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Ecosystem.Projects;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(ProjectsDomainSharedModule))]
public class ProjectsDomainModule : AbpModule { }
