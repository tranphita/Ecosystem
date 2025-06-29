using Microsoft.Extensions.Hosting;
using Projects;

namespace Ecosystem.AppHost;

internal class Program
{
    private static void Main(string[] args)
    {
        const string LaunchProfileName = "Aspire";
        var builder = DistributedApplication.CreateBuilder(args);

        // Sử dụng connection strings cho tất cả infrastructure services
        var adminDb = builder.AddConnectionString(EcosystemNames.AdministrationDb);
        var identityDb = builder.AddConnectionString(EcosystemNames.IdentityServiceDb);
        var projectsDb = builder.AddConnectionString(EcosystemNames.ProjectsDb);
        var saasDb = builder.AddConnectionString(EcosystemNames.SaaSDb);
        var rabbitMq = builder.AddConnectionString(EcosystemNames.RabbitMq);
        var redis = builder.AddConnectionString(EcosystemNames.Redis);
        var seq = builder.AddConnectionString(EcosystemNames.Seq);

        var migrator = builder
            .AddProject<Ecosystem_DbMigrator>(
                EcosystemNames.DbMigrator,
                launchProfileName: LaunchProfileName
            )
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(projectsDb)
            .WithReference(saasDb)
            .WithReference(seq);

        var admin = builder
            .AddProject<Ecosystem_Administration_HttpApi_Host>(
                EcosystemNames.AdministrationApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var identity = builder
            .AddProject<Ecosystem_IdentityService_HttpApi_Host>(
                EcosystemNames.IdentityServiceApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var saas = builder
            .AddProject<Ecosystem_SaaS_HttpApi_Host>(
                EcosystemNames.SaaSApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder
            .AddProject<Ecosystem_Projects_HttpApi_Host>(
                EcosystemNames.ProjectsApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(projectsDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var gateway = builder
            .AddProject<Ecosystem_Gateway>(EcosystemNames.Gateway, launchProfileName: LaunchProfileName)
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(admin)
            .WaitFor(identity)
            .WaitFor(saas);

        var authserver = builder
            .AddProject<Ecosystem_AuthServer>(
                EcosystemNames.AuthServer,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder.Build().Run();
    }
}
