using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Persistence.WriteDb.Data;
using Ecosystem.Persistence.WriteDb.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecosystem.Persistence.WriteDb;

/// <summary>
/// Triển khai UnitOfWork cho WriteDbContext.
/// Quản lý các repository và thực hiện lưu thay đổi vào cơ sở dữ liệu.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _context;
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;

    /// <summary>
    /// Khởi tạo UnitOfWork với WriteDbContext.
    /// </summary>
    /// <param name="context">DbContext dùng cho ghi dữ liệu.</param>
    public UnitOfWork(WriteDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Repository cho thực thể User, được khởi tạo khi cần (lazy loading).
    /// </summary>
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    /// <summary>
    /// Repository cho thực thể Role, được khởi tạo khi cần (lazy loading).
    /// </summary>
    public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);

    /// <summary>
    /// Lưu các thay đổi vào cơ sở dữ liệu.
    /// Dùng khi không có transaction hoặc muốn lưu thay đổi ngay lập tức.
    /// </summary>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>Số lượng bản ghi bị ảnh hưởng.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Bắt đầu một transaction mới trong cơ sở dữ liệu.
    /// </summary>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    /// <returns>Đối tượng transaction của Entity Framework Core.</returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Sử dụng trực tiếp API của EF Core để bắt đầu một transaction
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit (xác nhận) một transaction đã cho, đồng thời lưu các thay đổi vào cơ sở dữ liệu.
    /// Nếu có lỗi xảy ra, transaction sẽ được rollback.
    /// </summary>
    /// <param name="transaction">Đối tượng transaction cần commit.</param>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        try
        {
            // Lưu tất cả các thay đổi trong DbContext vào CSDL
            await SaveChangesAsync(cancellationToken);
            // Sau đó, commit transaction ở cấp độ CSDL
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Nếu có lỗi xảy ra trong quá trình commit, rollback ngay lập tức
            await RollbackTransactionAsync(transaction, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Rollback (hoàn tác) một transaction đã cho.
    /// </summary>
    /// <param name="transaction">Đối tượng transaction cần rollback.</param>
    /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        await transaction.RollbackAsync(cancellationToken);
    }
}