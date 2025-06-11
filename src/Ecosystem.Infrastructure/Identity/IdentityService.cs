using Ecosystem.Application.Abstractions.Identity;
using Ecosystem.Shared;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Ecosystem.Infrastructure.Identity;

/// <summary>
/// Dịch vụ định danh, thực hiện các thao tác liên quan đến người dùng với hệ thống IDP.
/// </summary>
internal sealed class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Khởi tạo dịch vụ định danh với HttpClient và cấu hình IDP.
    /// </summary>
    /// <param name="httpClient">HttpClient để gọi API IDP.</param>
    /// <param name="idpSettings">Cấu hình địa chỉ IDP.</param>
    public IdentityService(HttpClient httpClient, IOptions<IdpSettings> idpSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(idpSettings.Value.BaseUrl);
    }

    /// <summary>
    /// Đổi mật khẩu cho người dùng.
    /// </summary>
    /// <param name="authId">ID xác thực của người dùng.</param>
    /// <param name="currentPassword">Mật khẩu hiện tại.</param>
    /// <param name="newPassword">Mật khẩu mới.</param>
    /// <param name="cancellationToken">Token huỷ thao tác.</param>
    /// <returns>Kết quả thành công hoặc thất bại.</returns>
    public async Task<Result> ChangePasswordAsync(
        Guid authId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var requestPayload = new
        {
            currentPassword,
            newPassword
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/identity/users/{authId}/change-password",
            requestPayload,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
        var errorCode = errorResponse?.Error?.Code ?? "Identity.ChangePasswordFailed";
        var errorMessage = errorResponse?.Error?.Message ?? "Failed to change password at IDP.";

        return Result.Failure(new Error(errorCode, errorMessage));
    }

    /// <summary>
    /// Lớp nội bộ để deserialize lỗi từ IDP.
    /// </summary>
    private record ErrorResponse(ErrorDetails? Error);

    /// <summary>
    /// Chi tiết lỗi trả về từ IDP.
    /// </summary>
    private record ErrorDetails(string Code, string Message);
}
