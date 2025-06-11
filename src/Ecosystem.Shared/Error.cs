namespace Ecosystem.Shared;

/// <summary>
/// Đại diện cho một lỗi với mã lỗi và thông báo mô tả.
/// </summary>
/// <remarks>Kiểu này thường được sử dụng để đóng gói thông tin lỗi trong các ứng dụng, cung cấp cả mã lỗi có thể đọc được bởi máy và thông báo có thể đọc được bởi con người.</remarks>
/// <param name="Code">Mã lỗi</param>
/// <param name="Message">Thông báo lỗi</param>
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}