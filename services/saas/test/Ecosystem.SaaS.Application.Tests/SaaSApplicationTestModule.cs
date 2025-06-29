using Volo.Abp.Modularity;

namespace Ecosystem.SaaS;

[DependsOn(typeof(SaaSApplicationModule))]
[DependsOn(typeof(SaaSDomainTestModule))]
public class SaaSApplicationTestModule : AbpModule { }
