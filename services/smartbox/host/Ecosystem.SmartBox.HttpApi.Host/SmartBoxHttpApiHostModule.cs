using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ecosystem.SmartBox.EntityFrameworkCore;
using Ecosystem.IdentityService;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.VirtualFileSystem;

namespace Ecosystem.SmartBox;

[DependsOn(typeof(AbpAspNetCoreMvcUiMultiTenancyModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(SmartBoxApplicationModule))]
[DependsOn(typeof(SmartBoxEntityFrameworkCoreModule))]
[DependsOn(typeof(SmartBoxHttpApiModule))]
[DependsOn(typeof(EcosystemMicroserviceModule))]
[DependsOn(typeof(EcosystemServiceDefaultsModule))]
[DependsOn(typeof(AbpEventBusRabbitMqModule))]
[DependsOn(typeof(IdentityServiceHttpApiClientModule))]
public class SmartBoxHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.ConfigureMicroservice(EcosystemNames.SmartBoxApi);

        // Configure Identity Service HTTP Client
        Configure<AbpRemoteServiceOptions>(options =>
        {
            options.RemoteServices.Default.BaseUrl = configuration["RemoteServices:IdentityService:BaseUrl"] ?? "https://localhost:44302/";
        });

        // Configure RabbitMQ Event Bus
        Configure<AbpRabbitMqEventBusOptions>(options =>
        {
            options.ConnectionName = "rabbitmq";
            options.ClientName = configuration["RabbitMQ:EventBus:ClientName"] ?? "Ecosystem.SmartBox";
            options.ExchangeName = configuration["RabbitMQ:EventBus:ExchangeName"] ?? "Ecosystem";
        });

        // Configure Health Checks (simplified for now)
        context.Services.AddHealthChecks()
            .AddRabbitMQ(configuration.GetConnectionString("rabbitmq") ?? "amqp://guest:guest@localhost:5672", name: "rabbitmq")
            .AddNpgSql(configuration.GetConnectionString("EcosystemSmartBoxDb") ?? "Host=localhost;Port=5432;Database=ecosystem_smartbox;Username=postgres;Password=postgres", name: "database");

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<SmartBoxDomainSharedModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}Ecosystem.SmartBox.Domain.Shared",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<SmartBoxDomainModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}Ecosystem.SmartBox.Domain",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<SmartBoxApplicationContractsModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}Ecosystem.SmartBox.Application.Contracts",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<SmartBoxApplicationModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}Ecosystem.SmartBox.Application",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
            });
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        app.UseMultiTenancy();

        app.UseAbpRequestLocalization();
        app.UseAuthorization();

        // Add Health Checks endpoint
        app.UseHealthChecks("/health");

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBox API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            options.OAuthScopes("SmartBox");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
