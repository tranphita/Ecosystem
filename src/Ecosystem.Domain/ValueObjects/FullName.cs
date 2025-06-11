using Ecosystem.Shared;

namespace Ecosystem.Domain.ValueObjects;

/// <summary>
/// Giá trị đối tượng đại diện cho họ và tên đầy đủ của một cá nhân.
/// </summary>
public sealed record FullName
{
    /// <summary>
    /// Họ của cá nhân.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Tên của cá nhân.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Khởi tạo một đối tượng <see cref="FullName"/> với họ và tên.
    /// </summary>
    /// <param name="firstName">Họ</param>
    /// <param name="lastName">Tên</param>
    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// Tạo mới một đối tượng <see cref="FullName"/> từ họ và tên.
    /// </summary>
    /// <param name="firstName">Họ</param>
    /// <param name="lastName">Tên</param>
    /// <returns>Kết quả thành công hoặc thất bại kèm thông tin lỗi.</returns>
    public static Result<FullName> Create(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<FullName>.Failure(new Error("FullName.EmptyFirstName", "First name is empty."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<FullName>.Failure(new Error("FullName.EmptyLastName", "Last name is empty."));
        }

        var fullName = new FullName(firstName, lastName);
        return Result<FullName>.Success(fullName);
    }

    /// <summary>
    /// Trả về họ và tên dưới dạng chuỗi.
    /// </summary>
    /// <returns>Chuỗi họ và tên.</returns>
    public override string ToString() => $"{FirstName} {LastName}";
}