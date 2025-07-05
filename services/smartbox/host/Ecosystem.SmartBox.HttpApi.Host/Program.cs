using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Ecosystem.Administration.EntityFrameworkCore;
using Ecosystem.SmartBox.EntityFrameworkCore;

namespace Ecosystem.SmartBox;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        EcosystemLogging.Initialize();

        try
        {
            Log.Information("Starting web host.");

            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();
            builder.AddSharedEndpoints();

            builder.AddNpgsqlDbContext<AdministrationDbContext>(
                connectionName: EcosystemNames.AdministrationDb,
                configure => configure.DisableRetry = true
            );
            builder.AddNpgsqlDbContext<SmartBoxDbContext>(
                connectionName: EcosystemNames.SmartBoxDb,
                configure => configure.DisableRetry = true
            );

            builder.Host.AddAppSettingsSecretsJson().UseAutofac().UseSerilog();

            await builder.AddApplicationAsync<SmartBoxHttpApiHostModule>();

            var app = builder.Build();

            await app.InitializeApplicationAsync();

            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
