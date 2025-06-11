using Ecosystem.Application.Abstractions.Identity;
using Ecosystem.Application.Abstractions.Security;
using Ecosystem.Application.Caching;
using Ecosystem.Infrastructure.Caching;
using Ecosystem.Infrastructure.Identity;
using Ecosystem.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Ecosystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Cấu hình cho JWT
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Cấu hình cho Identity Service
        services.Configure<IdpSettings>(configuration.GetSection(IdpSettings.SectionName));
        services.AddHttpClient<IIdentityService, IdentityService>();

        // Cấu hình Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        services.AddScoped<ICacheService, RedisCacheService>();

        // Cấu hình MassTransit (ví dụ cơ bản)
        // services.AddMassTransit(busConfigurator =>
        // {
        //     busConfigurator.SetKebabCaseEndpointNameFormatter();
        //     // Tự động tìm consumers trong assembly này
        //     busConfigurator.AddConsumers(typeof(DependencyInjection).Assembly);
        //     
        //     busConfigurator.UsingRabbitMq((context, cfg) =>
        //     {
        //         cfg.Host(configuration.GetConnectionString("RabbitMQ"));
        //         cfg.ConfigureEndpoints(context);
        //     });
        // });

        // Cấu hình Hangfire (ví dụ cơ bản)
        // services.AddHangfire(config => 
        //     config.UsePostgreSqlStorage(c => 
        //         c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));
        // services.AddHangfireServer();

        return services;
    }
}
