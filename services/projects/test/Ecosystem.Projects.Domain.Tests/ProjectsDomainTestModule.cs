using Ecosystem.Projects.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ecosystem.Projects;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(typeof(ProjectsEntityFrameworkCoreTestModule))]
public class ProjectsDomainTestModule : AbpModule { }
