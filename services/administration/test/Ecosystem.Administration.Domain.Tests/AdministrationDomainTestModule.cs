using Ecosystem.Administration.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ecosystem.Administration;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(typeof(AdministrationEntityFrameworkCoreTestModule))]
public class AdministrationDomainTestModule : AbpModule { }
