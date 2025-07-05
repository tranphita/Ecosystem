using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

[ConnectionStringName(EcosystemNames.SmartBoxDb)]
public class SmartBoxDbContext(DbContextOptions<SmartBoxDbContext> options)
    : AbpDbContext<SmartBoxDbContext>(options),
        ISmartBoxDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSmartBox();
    }
}
