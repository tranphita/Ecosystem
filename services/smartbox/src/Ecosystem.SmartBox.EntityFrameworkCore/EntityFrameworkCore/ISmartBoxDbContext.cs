using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

[ConnectionStringName(EcosystemNames.SmartBoxDb)]
public interface ISmartBoxDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
