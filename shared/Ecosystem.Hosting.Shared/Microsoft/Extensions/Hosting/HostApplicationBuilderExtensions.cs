using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddSharedEndpoints(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMQClient(
            connectionName: EcosystemNames.RabbitMq,
            action =>
                action.ConnectionString = builder.Configuration.GetConnectionString(
                    EcosystemNames.RabbitMq
                )
        );
        builder.AddRedisDistributedCache(connectionName: EcosystemNames.Redis);
        builder.AddSeqEndpoint(connectionName: EcosystemNames.Seq);

        return builder;
    }
}
