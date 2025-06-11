namespace Ecosystem.Domain.Enums
{
    /// <summary>
    /// Enum Permission định nghĩa các quyền truy cập trong hệ thống.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// Không có quyền (giá trị mặc định/lỗi).
        /// </summary>
        None = 0,

        // Nhóm quyền cho Users
        ReadUsers = 1,
        CreateUsers = 2,
        UpdateUsers = 3,
        DeleteUsers = 4,

        // Nhóm quyền cho Orders
        ReadOrders = 11,
        CreateOrders = 12,
        UpdateOrders = 13,

        // Nhóm quyền cho Roles (quản trị hệ thống)
        ReadRoles = 21,
        UpdateRoles = 22,
    }
}
