using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(SmartBoxDomainSharedModule))]
public class SmartBoxDomainModule : AbpModule { }
