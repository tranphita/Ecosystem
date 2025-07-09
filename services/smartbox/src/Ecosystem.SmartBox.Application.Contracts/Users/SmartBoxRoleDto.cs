using System;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho vai trò SmartBox
/// </summary>
public class SmartBoxRoleDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// Tên vai trò
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả vai trò
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Vai trò hệ thống (không thể xóa)
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Thứ tự hiển thị
    /// </summary>
    public int DisplayOrder { get; set; }
} 