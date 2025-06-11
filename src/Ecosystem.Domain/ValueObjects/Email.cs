using Ecosystem.Shared;
using System.Text.RegularExpressions;

namespace Ecosystem.Domain.ValueObjects;

/// <summary>
/// Đại diện cho giá trị email trong hệ thống.
/// </summary>
public sealed partial record Email
{
    private const string EmailRegexPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    /// <summary>
    /// Giá trị email.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Khởi tạo một đối tượng <see cref="Email"/> với giá trị email đã cho.
    /// </summary>
    /// <param name="value">Giá trị email.</param>
    private Email(string value) => Value = value;

    /// <summary>
    /// Tạo mới một đối tượng <see cref="Email"/> từ chuỗi email đầu vào.
    /// </summary>
    /// <param name="email">Chuỗi email cần kiểm tra và tạo.</param>
    /// <returns>
    /// Kết quả thành công với đối tượng <see cref="Email"/> hợp lệ hoặc thất bại với thông tin lỗi.
    /// </returns>
    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Email>.Failure(new Error("Email.Empty", "Email is empty."));
        }

        if (!Regex.IsMatch(email, EmailRegexPattern))
        {
            return Result<Email>.Failure(new Error("Email.InvalidFormat", "Email format is invalid."));
        }

        return Result<Email>.Success(new Email(email));
    }
}
