using Ecosystem.Domain.Enums;
using Ecosystem.Domain.Primitives;
using Ecosystem.Shared;

namespace Ecosystem.Domain.Aggregates.Role;

/// <summary>
/// Đại diện cho một vai trò (Role) trong hệ thống, chứa tên và tập hợp các quyền (Permission) liên quan.
/// </summary>
public sealed class Role : AggregateRoot<RoleId>
{
    /// <summary>
    /// Tập hợp các quyền (Permission) của vai trò.
    /// </summary>
    private readonly HashSet<Permission> _permissions = [];

#pragma warning disable CS8618
    private Role() : base(default) { } // CONSTRUCTOR RỖNG DÀNH RIÊNG CHO EF CORE
#pragma warning restore CS8618

    /// <summary>
    /// Khởi tạo một vai trò mới với Id và tên.
    /// </summary>
    /// <param name="id">Định danh vai trò.</param>
    /// <param name="name">Tên vai trò.</param>
    private Role(RoleId id, string name) : base(id)
    {
        Name = name;
    }

    /// <summary>
    /// Tên của vai trò.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Danh sách chỉ đọc các quyền của vai trò.
    /// </summary>
    public IReadOnlyCollection<Permission> Permissions => _permissions;

    /// <summary>
    /// Tạo mới một vai trò với tên được cung cấp.
    /// </summary>
    /// <param name="name">Tên vai trò.</param>
    /// <returns>Kết quả thành công hoặc thất bại kèm thông tin lỗi.</returns>
    public static Result<Role> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Role>.Failure(new Error("Role.EmptyName", "Role name cannot be empty."));
        }

        return new Role(RoleId.New(), name);
    }

    /// <summary>
    /// Thêm một quyền vào vai trò.
    /// </summary>
    /// <param name="permission">Quyền cần thêm.</param>
    public void AddPermission(Permission permission)
    {
        _permissions.Add(permission);
    }

    /// <summary>
    /// Thêm nhiều quyền vào vai trò.
    /// </summary>
    /// <param name="permissions">Danh sách quyền cần thêm.</param>
    public void AddPermissions(IEnumerable<Permission> permissions)
    {
        foreach (var permission in permissions)
        {
            AddPermission(permission);
        }
    }
}