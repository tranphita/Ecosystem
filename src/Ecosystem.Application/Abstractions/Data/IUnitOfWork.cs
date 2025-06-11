using Microsoft.EntityFrameworkCore.Storage;

namespace Ecosystem.Application.Abstractions.Data;
public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }

    IRoleRepository RoleRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Bắt đầu một giao dịch CSDL mới.
    /// </summary>
    /// <returns>Một đối tượng đại diện cho transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit một giao dịch đã cho.
    /// </summary>
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback một giao dịch đã cho.
    /// </summary>
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
}