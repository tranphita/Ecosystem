using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Persistence.WriteDb.Data;
using Ecosystem.Persistence.WriteDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecosystem.Persistence.WriteDb;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Đăng ký DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        // 2. Đăng ký Unit of Work và các Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Bạn có thể đăng ký các repository khác của tầng Persistence ở đây

        return services;
    }
}