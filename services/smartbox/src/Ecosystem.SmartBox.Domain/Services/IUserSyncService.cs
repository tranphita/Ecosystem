using System;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;

namespace Ecosystem.SmartBox.Services;

/// <summary>
/// Service interface cho việc đồng bộ user từ AuthServer
/// </summary>
public interface IUserSyncService
{
    /// <summary>
    /// Tạo hoặc cập nhật user từ thông tin authentication claims
    /// </summary>
    Task<SmartBoxUser> SyncUserAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cập nhật thời gian đăng nhập cuối cho user
    /// </summary>
    Task UpdateLastLoginTimeAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy hoặc tạo user từ AuthUserId
    /// </summary>
    Task<SmartBoxUser> GetOrCreateUserAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra user có tồn tại không
    /// </summary>
    Task<bool> IsUserExistAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cập nhật thông tin cơ bản của user
    /// </summary>
    Task UpdateUserBasicInfoAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default
    );
} 