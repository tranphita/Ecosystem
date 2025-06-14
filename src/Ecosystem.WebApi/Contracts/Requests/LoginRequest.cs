using System.ComponentModel.DataAnnotations;

namespace Ecosystem.WebApi.Contracts.Requests;

/// <summary>
/// DTO nhận thông tin đăng nhập từ client.
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// Tên đăng nhập hoặc email.
    /// </summary>
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    [StringLength(255, ErrorMessage = "Tên đăng nhập không vượt quá 255 ký tự.")]
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Mật khẩu đăng nhập.
    /// </summary>
    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Mật khẩu phải từ 1 đến 100 ký tự.")]
    public string Password { get; init; } = string.Empty;
} 