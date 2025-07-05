using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Ecosystem.SaaS.EntityFrameworkCore;

[ConnectionStringName(EcosystemNames.SaaSDb)]
public interface ISaaSDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
