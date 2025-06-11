using Ecosystem.Application.Abstractions.Security;
using Ecosystem.Domain.Aggregates.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecosystem.Infrastructure.Security;

/// <summary>
/// Triển khai bộ sinh JWT token cho người dùng.
/// </summary>
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="JwtTokenGenerator"/> với cấu hình JWT.
    /// </summary>
    /// <param name="jwtOptions">Tùy chọn cấu hình JWT.</param>
    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    /// <summary>
    /// Sinh ra chuỗi JWT token cho người dùng.
    /// </summary>
    /// <param name="user">Đối tượng người dùng.</param>
    /// <returns>Chuỗi JWT token hợp lệ.</returns>
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            // Các claims chuẩn
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Các claims tùy chỉnh
            new("authId", user.AuthId.ToString()),
            new("fullName", user.FullName.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
