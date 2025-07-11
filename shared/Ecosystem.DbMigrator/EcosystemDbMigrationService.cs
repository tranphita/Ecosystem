using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Ecosystem.Administration.EntityFrameworkCore;
using Ecosystem.IdentityService.EntityFrameworkCore;
using Ecosystem.SmartBox.EntityFrameworkCore;
using Ecosystem.SaaS.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;

namespace Ecosystem.DbMigrator;

public class EcosystemDbMigrationService(
    ILogger<EcosystemDbMigrationService> logger,
    ITenantRepository tenantRepository,
    IDataSeeder dataSeeder,
    ICurrentTenant currentTenant,
    IUnitOfWorkManager unitOfWorkManager
) : ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IDataSeeder _dataSeeder = dataSeeder;
    private readonly ILogger<EcosystemDbMigrationService> _logger = logger;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await CreateDatabasesAsync(cancellationToken);
        await MigrateHostAsync(cancellationToken);
        await MigrateTenantsAsync(cancellationToken);
    }

    private async Task CreateDatabasesAsync(CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkManager.Begin(true);

        await EnsureDatabaseAsync<SaaSDbContext>(cancellationToken);
        await EnsureDatabaseAsync<AdministrationDbContext>(cancellationToken);
        await EnsureDatabaseAsync<IdentityServiceDbContext>(cancellationToken);
        await EnsureDatabaseAsync<SmartBoxDbContext>(cancellationToken);

        await uow.CompleteAsync(cancellationToken);
    }

    private async Task MigrateHostAsync(CancellationToken cancellationToken)
    {
        await MigrateDatabasesAsync(null, cancellationToken);
        await SeedDataAsync(null);
    }

    private async Task MigrateTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetListAsync(
            includeDetails: true,
            cancellationToken: cancellationToken
        );
        var migratedDatabaseSchemas = new HashSet<string>();

        foreach (var tenant in tenants)
        {
            using (_currentTenant.Change(tenant.Id))
            {
                // Database schema migration
                var connectionString = tenant.FindDefaultConnectionString();
                if (
                    !connectionString.IsNullOrWhiteSpace()
                    && //tenant has a separate database
                    !migratedDatabaseSchemas.Contains(connectionString)
                )
                {
                    await MigrateDatabasesAsync(tenant, cancellationToken);
                    migratedDatabaseSchemas.AddIfNotContains(connectionString);
                }

                //Seed data
                await SeedDataAsync(tenant);
            }
        }
    }

    private async Task EnsureDatabaseAsync<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext, IEfCoreDbContext
    {
        var dbContext = await _unitOfWorkManager
            .Current!.ServiceProvider.GetRequiredService<IDbContextProvider<TDbContext>>()
            .GetDbContextAsync();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private async Task MigrateDatabasesAsync(Tenant? tenant, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkManager.Begin(true);

        if (tenant is null)
        {
            /* SaaS schema should only be available in the host side */
            await MigrateDatabaseAsync<SaaSDbContext>(cancellationToken);
        }

        await MigrateDatabaseAsync<AdministrationDbContext>(cancellationToken);
        await MigrateDatabaseAsync<IdentityServiceDbContext>(cancellationToken);
        await MigrateDatabaseAsync<SmartBoxDbContext>(cancellationToken);

        await uow.CompleteAsync(cancellationToken);
    }

    private async Task MigrateDatabaseAsync<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext, IEfCoreDbContext
    {
        var name = typeof(TDbContext).Name.RemovePostFix("DbContext");
        var dbContext = await _unitOfWorkManager
            .Current!.ServiceProvider.GetRequiredService<IDbContextProvider<TDbContext>>()
            .GetDbContextAsync();

        await ApplyMigrationAsync(dbContext, cancellationToken);
    }

    private static Task ApplyMigrationAsync<TDbContext>(
        TDbContext dbContext,
        CancellationToken cancellationToken
    )
        where TDbContext : DbContext, IEfCoreDbContext
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private Task SeedDataAsync(Tenant? tenant)
    {
        return _dataSeeder.SeedAsync(
            new DataSeedContext(tenant?.Id)
                .WithProperty(
                    IdentityDataSeedContributor.AdminEmailPropertyName,
                    IdentityDataSeedContributor.AdminEmailDefaultValue
                )
                .WithProperty(
                    IdentityDataSeedContributor.AdminPasswordPropertyName,
                    IdentityDataSeedContributor.AdminPasswordDefaultValue
                )
        );
    }
}
