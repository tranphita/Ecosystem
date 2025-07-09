using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Ecosystem.SmartBox.Entities;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

[ConnectionStringName(EcosystemNames.SmartBoxDb)]
public interface ISmartBoxDbContext : IEfCoreDbContext
{
    /* DbSets cho các entities */
    DbSet<Company> Companies { get; }
    DbSet<SmartBoxUser> SmartBoxUsers { get; }
    DbSet<SmartBoxRole> SmartBoxRoles { get; }
    DbSet<SmartBoxUserRole> SmartBoxUserRoles { get; }
}
