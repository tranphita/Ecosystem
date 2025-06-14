namespace Ecosystem.Application.Features.Authentication.DTOs;

/// <summary>
/// DTO chứa thông tin token trả về từ Identity Provider bên ngoài.
/// </summary>
/// <param name="AccessToken">Access token để gọi API</param>
/// <param name="RefreshToken">Refresh token để làm mới access token</param>
/// <param name="TokenType">Loại token (thường là "Bearer")</param>
/// <param name="ExpiresIn">Thời gian sống của token (giây)</param>
public sealed record IdpTokenResponse(
    string AccessToken,
    string? RefreshToken,
    string TokenType,
    int ExpiresIn)
{
    public override string ToString() =>
        $"{nameof(IdpTokenResponse)}: {{ TokenType = {TokenType}, ExpiresIn = {ExpiresIn} }}";
} 