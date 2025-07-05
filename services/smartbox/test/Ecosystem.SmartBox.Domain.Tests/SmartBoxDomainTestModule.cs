using Ecosystem.SmartBox.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ecosystem.SmartBox;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(typeof(SmartBoxEntityFrameworkCoreTestModule))]
public class SmartBoxDomainTestModule : AbpModule { }
