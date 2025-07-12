using System;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;

namespace Ecosystem.SmartBox.Services;

/// <summary>
/// Service interface cho vi?c ??ng b? user 2 chi?u gi?a SmartBox và Identity Service
/// </summary>
public interface IUserBidirectionalSyncService
{
    /// <summary>
    /// T?o user trong Identity Service và ??ng b? v?i SmartBox
    /// </summary>
    Task<(Guid AuthUserId, SmartBoxUser SmartBoxUser)> CreateUserInIdentityAsync(
        string userName,
        string email,
        string password,
        string? fullName = null,
        string? phoneNumber = null,
        bool isActive = true,
        bool requirePasswordChange = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// C?p nh?t user trong Identity Service t? thông tin SmartBox
    /// </summary>
    Task SyncSmartBoxUserToIdentityAsync(
        SmartBoxUser smartBoxUser,
        bool syncBasicInfo = true,
        bool syncStatus = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// ??ng b? user t? Identity Service v? SmartBox
    /// </summary>
    Task<SmartBoxUser> SyncIdentityUserToSmartBoxAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        string? phoneNumber = null,
        bool isActive = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Xóa user trong Identity Service khi xóa trong SmartBox
    /// </summary>
    Task DeleteUserFromIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kích ho?t/vô hi?u hóa user trong Identity Service
    /// </summary>
    Task SetUserActiveInIdentityAsync(
        Guid authUserId,
        bool isActive,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Ki?m tra user có t?n t?i trong Identity Service không
    /// </summary>
    Task<bool> IsUserExistInIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// L?y thông tin user t? Identity Service
    /// </summary>
    Task<(string UserName, string Email, string? FullName, string? PhoneNumber, bool IsActive)?> GetUserFromIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default
    );
}