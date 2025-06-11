using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Persistence.ReadDb.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Ecosystem.Persistence.ReadDb;

public static class DependencyInjection
{
    public static IServiceCollection AddReadPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Đăng ký IDbConnection
        // Chúng ta đăng ký nó như một Transient service. Mỗi khi một dịch vụ cần IDbConnection,
        // một kết nối mới sẽ được tạo, mở ra và cung cấp.
        // Điều này rất phù hợp với Dapper để đảm bảo các kết nối được quản lý đúng cách.
        services.AddTransient<IDbConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var connection = new NpgsqlConnection(connectionString);
            connection.Open(); // Mở kết nối sẵn sàng để sử dụng
            return connection;
        });

        // 2. Đăng ký UserReadRepository
        services.AddScoped<IUserReadRepository, UserReadRepository>();

        return services;
    }
}