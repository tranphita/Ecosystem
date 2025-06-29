namespace Microsoft.Extensions.Hosting;

public static class EcosystemNames
{
    public const string Ecosystem = "Ecosystem";

    public const string Postgres = "postgres";
    public const string RabbitMq = "rabbitmq";
    public const string Redis = "redis";
    public const string Seq = "seq";

    public const string AdministrationApi = "EcosystemAdministration";
    public const string AdministrationDb = "EcosystemAdministrationDb";

    public const string AuthServer = "EcosystemAuthServer";
    public const string AuthServerDb = "EcosystemAuthServerDb";

    public const string DbMigrator = "EcosystemDbMigrator";

    public const string Gateway = "EcosystemGateway";

    public const string IdentityServiceApi = "EcosystemIdentityService";
    public const string IdentityServiceDb = "EcosystemIdentityServiceDb";

    public const string ProjectsApi = "EcosystemProjects";
    public const string ProjectsDb = "EcosystemProjectsDb";

    public const string SaaSApi = "EcosystemSaaS";
    public const string SaaSDb = "EcosystemSaaSDb";

    public const string WebAppApi = "EcosystemWebApp";
    public const string WebAppClient = "EcosystemWebAppClient";
    public const string WebAppDb = "EcosystemWebAppDb";
}
