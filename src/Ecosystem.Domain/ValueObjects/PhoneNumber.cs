using Ecosystem.Shared;
using System.Text.RegularExpressions;

namespace Ecosystem.Domain.ValueObjects;

public sealed record PhoneNumber
{
    // Một regex đơn giản để ví dụ, bạn có thể dùng một thư viện chuyên dụng như libphonenumber-csharp
    private const string PhoneRegexPattern = @"^\+?[1-9]\d{1,14}$";

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            // Cho phép số điện thoại có thể null hoặc empty
            return Result<PhoneNumber>.Success(new PhoneNumber(string.Empty));
        }

        if (!Regex.IsMatch(phoneNumber, PhoneRegexPattern))
        {
            return Result<PhoneNumber>.Failure(new Error("PhoneNumber.InvalidFormat", "Phone number format is invalid."));
        }

        return Result<PhoneNumber>.Success(new PhoneNumber(phoneNumber));
    }
}