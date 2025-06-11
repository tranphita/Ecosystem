using Dapper;
using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Features.Users.DTOs;
using System.Data;

namespace Ecosystem.Persistence.ReadDb.Repositories;

/// <summary>
/// Repository đọc thông tin người dùng từ cơ sở dữ liệu.
/// </summary>
internal sealed class UserReadRepository : IUserReadRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// Khởi tạo UserReadRepository với kết nối cơ sở dữ liệu.
    /// </summary>
    /// <param name="connection">Kết nối cơ sở dữ liệu.</param>
    public UserReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Lấy thông tin người dùng theo Id.
    /// </summary>
    /// <param name="id">Id của người dùng.</param>
    /// <param name="cancellationToken">Token huỷ thao tác bất đồng bộ.</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy.</returns>
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                id AS Id,
                first_name || ' ' || last_name AS FullName,
                email AS Email,
                is_active AS IsActive
            FROM
                users
            WHERE
                id = @UserId
            """;

        return await _connection.QueryFirstOrDefaultAsync<UserDto>(
            sql,
            new { UserId = id });
    }
}
