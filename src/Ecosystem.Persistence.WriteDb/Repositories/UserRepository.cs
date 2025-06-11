using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.ValueObjects;
using Ecosystem.Persistence.WriteDb.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecosystem.Persistence.WriteDb.Repositories;

/// <summary>
/// Repository cho thực thể User, cung cấp các phương thức thao tác với bảng Users trong cơ sở dữ liệu ghi (WriteDbContext).
/// </summary>
internal sealed class UserRepository : IUserRepository
{
    private readonly WriteDbContext _context;

    /// <summary>
    /// Khởi tạo UserRepository với WriteDbContext.
    /// </summary>
    /// <param name="context">DbContext dùng cho ghi dữ liệu.</param>
    public UserRepository(WriteDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Thêm một user mới vào cơ sở dữ liệu.
    /// </summary>
    /// <param name="user">Thực thể User cần thêm.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    /// <summary>
    /// Lấy user theo AuthId.
    /// </summary>
    /// <param name="authId">Định danh xác thực của user.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>User nếu tìm thấy, ngược lại trả về null.</returns>
    public async Task<User?> GetByAuthIdAsync(Guid authId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.AuthId == authId, cancellationToken);
    }

    /// <summary>
    /// Lấy user theo email.
    /// </summary>
    /// <param name="email">Email của user.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>User nếu tìm thấy, ngược lại trả về null.</returns>
    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Lấy user theo UserId.
    /// </summary>
    /// <param name="id">Định danh UserId.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>User nếu tìm thấy, ngược lại trả về null.</returns>
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Lấy user theo tên đăng nhập (username).
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}
