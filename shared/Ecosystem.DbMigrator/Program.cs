using Serilog;
using Ecosystem.Administration.EntityFrameworkCore;
using Ecosystem.Projects.EntityFrameworkCore;
using Ecosystem.SaaS.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Ecosystem.DbMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        EcosystemLogging.Initialize();

        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.AddNpgsqlDbContext<AdministrationDbContext>(
            connectionName: EcosystemNames.AdministrationDb
        );
        builder.AddNpgsqlDbContext<IdentityDbContext>(connectionName: EcosystemNames.IdentityServiceDb);
        builder.AddNpgsqlDbContext<SaaSDbContext>(connectionName: EcosystemNames.SaaSDb);
        builder.AddNpgsqlDbContext<ProjectsDbContext>(connectionName: EcosystemNames.ProjectsDb);

        builder.Configuration.AddAppSettingsSecretsJson();

        builder.Logging.AddSerilog();

        builder.Services.AddHostedService<DbMigratorHostedService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
