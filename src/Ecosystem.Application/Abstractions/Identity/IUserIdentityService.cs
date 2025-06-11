using Ecosystem.Shared;

namespace Ecosystem.Application.Abstractions.Identity;

/// <summary>
/// Interface để tương tác với hệ thống Identity Provider bên ngoài.
/// </summary>
public interface IUserIdentityService
{
    /// <summary>
    /// Yêu cầu thay đổi mật khẩu người dùng tại Identity Provider.
    /// </summary>
    /// <param name="authId">Auth ID của người dùng.</param>
    /// <param name="newPassword">Mật khẩu mới.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>Kết quả thao tác.</returns>
    Task<Result> ChangePasswordAsync(Guid authId, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy thông tin người dùng từ Identity Provider.
    /// </summary>
    /// <param name="authId">Auth ID của người dùng.</param>
    /// <param name="cancellationToken">Token hủy bỏ.</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy.</returns>
    Task<UserIdentityInfo?> GetUserInfoAsync(Guid authId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Thông tin người dùng từ Identity Provider.
/// </summary>
public record UserIdentityInfo(
    Guid AuthId,
    string Username,
    string Email,
    string FirstName,
    string LastName); 