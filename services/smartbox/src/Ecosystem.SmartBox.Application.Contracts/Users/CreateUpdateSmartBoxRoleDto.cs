using System.ComponentModel.DataAnnotations;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho tạo và cập nhật vai trò SmartBox
/// </summary>
public class CreateUpdateSmartBoxRoleDto
{
    /// <summary>
    /// Tên vai trò
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả vai trò
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Thứ tự hiển thị
    /// </summary>
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; } = 0;
} 