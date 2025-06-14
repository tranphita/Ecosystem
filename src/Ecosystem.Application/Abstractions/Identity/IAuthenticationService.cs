using Ecosystem.Application.Features.Authentication.DTOs;
using Ecosystem.Shared;

namespace Ecosystem.Application.Abstractions.Identity;

/// <summary>
/// Service xác thực người dùng với hệ thống Identity Provider bên ngoài.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Xác thực thông tin đăng nhập với IDP (theo ROPC flow).
    /// </summary>
    /// <param name="username">Tên đăng nhập hoặc email</param>
    /// <param name="password">Mật khẩu người dùng</param>
    /// <param name="cancellationToken">Token huỷ thao tác</param>
    /// <returns>Kết quả chứa token hoặc lỗi</returns>
    Task<Result<IdpTokenResponse>> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy thông tin người dùng từ IDP bằng access token.
    /// </summary>
    /// <param name="accessToken">Access token từ IDP</param>
    /// <param name="cancellationToken">Token huỷ thao tác</param>
    /// <returns>Kết quả chứa thông tin user hoặc lỗi</returns>
    Task<Result<UserIdentityInfo>> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
} 