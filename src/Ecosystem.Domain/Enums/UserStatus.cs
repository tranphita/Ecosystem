namespace Ecosystem.Domain.Enums;

public enum UserStatus
{
    /// <summary>
    /// Tài khoản đang hoạt động bình thường.
    /// </summary>
    Active,

    /// <summary>
    /// Tài khoản mới tạo, đang chờ người dùng xác thực (ví dụ: qua email).
    /// </summary>
    PendingVerification,

    /// <summary>
    /// Tài khoản bị quản trị viên tạm đình chỉ, không thể đăng nhập.
    /// </summary>
    Suspended,

    /// <summary>
    /// Tài khoản bị khóa tạm thời (ví dụ: do nhập sai mật khẩu quá nhiều lần).
    /// </summary>
    LockedOut
}
