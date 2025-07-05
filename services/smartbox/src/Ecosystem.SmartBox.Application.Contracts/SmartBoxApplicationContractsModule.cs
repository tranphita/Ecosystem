using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(SmartBoxDomainSharedModule))]
[DependsOn(typeof(AbpDddApplicationContractsModule))]
[DependsOn(typeof(AbpAuthorizationModule))]
public class SmartBoxApplicationContractsModule : AbpModule { }
