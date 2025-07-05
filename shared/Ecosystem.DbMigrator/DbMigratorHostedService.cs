using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace Ecosystem.DbMigrator;

public class DbMigratorHostedService(
    IHostApplicationLifetime hostApplicationLifetime,
    IConfiguration configuration
) : IHostedService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var application = await AbpApplicationFactory.CreateAsync<EcosystemDbMigratorModule>(
            options =>
            {
                options.Services.ReplaceConfiguration(_configuration);
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
            }
        );

        await application.InitializeAsync();

        await application
            .ServiceProvider.GetRequiredService<EcosystemDbMigrationService>()
            .MigrateAsync(cancellationToken);

        await application.ShutdownAsync();

        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
