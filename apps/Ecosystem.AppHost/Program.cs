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
        var smartBoxDb = builder.AddConnectionString(EcosystemNames.SmartBoxDb);
        var saasDb = builder.AddConnectionString(EcosystemNames.SaaSDb);
        var rabbitMq = builder.AddConnectionString(EcosystemNames.RabbitMq);
        var redis = builder.AddConnectionString(EcosystemNames.Redis);
        var seq = builder.AddConnectionString(EcosystemNames.Seq);

        // Thêm các dịch vụ cơ sở hạ tầng
        var migrator = builder
            .AddProject<Ecosystem_DbMigrator>(
                EcosystemNames.DbMigrator,
                launchProfileName: LaunchProfileName
            )
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(smartBoxDb)
            .WithReference(saasDb)
            .WithReference(seq);

        // Thêm Administration API
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

        // Thêm Identity Service API
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

        // Thêm SaaS API
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

        // Thêm SmartBox API
        builder
            .AddProject<Ecosystem_SmartBox_HttpApi_Host>(
                EcosystemNames.SmartBoxApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(smartBoxDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        // Thêm Gateway
        builder
            .AddProject<Ecosystem_Gateway>(EcosystemNames.Gateway, launchProfileName: LaunchProfileName)
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(admin)
            .WaitFor(identity)
            .WaitFor(saas);

        // Thêm AuthServer 
        builder
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
