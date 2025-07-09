using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Ecosystem.SmartBox.Entities;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

[ConnectionStringName(EcosystemNames.SmartBoxDb)]
public class SmartBoxDbContext(DbContextOptions<SmartBoxDbContext> options)
    : AbpDbContext<SmartBoxDbContext>(options),
        ISmartBoxDbContext
{
    /* Thêm DbSet properties cho các entities mới */
    public DbSet<Company> Companies { get; set; }
    public DbSet<SmartBoxUser> SmartBoxUsers { get; set; }
    public DbSet<SmartBoxRole> SmartBoxRoles { get; set; }
    public DbSet<SmartBoxUserRole> SmartBoxUserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSmartBox();
    }
}
