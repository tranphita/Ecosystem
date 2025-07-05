using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(SmartBoxApplicationModule))]
[DependsOn(typeof(SmartBoxDomainTestModule))]
public class SmartBoxApplicationTestModule : AbpModule { }
