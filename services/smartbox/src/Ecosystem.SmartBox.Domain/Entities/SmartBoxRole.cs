using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Ecosystem.SmartBox.Entities;

/// <summary>
/// Entity quản lý vai trò trong SmartBox
/// </summary>
public class SmartBoxRole : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Tên vai trò
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị
    /// </summary>
    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả vai trò
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Vai trò hệ thống (không thể xóa)
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Thứ tự hiển thị
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    protected SmartBoxRole() { }

    public SmartBoxRole(
        Guid id,
        string name,
        string displayName,
        string? description = null,
        bool isSystem = false,
        Guid? tenantId = null
    ) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        IsSystem = isSystem;
        TenantId = tenantId;
    }
} 