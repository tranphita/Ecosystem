using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Ecosystem.SmartBox.Entities;

/// <summary>
/// Entity liên kết Many-to-Many giữa User và Role
/// </summary>
public class SmartBoxUserRole : Entity, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// ID người dùng
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// ID vai trò
    /// </summary>
    [Required]
    public Guid RoleId { get; set; }

    /// <summary>
    /// Ngày gán vai trò
    /// </summary>
    public DateTime AssignedDate { get; set; }

    /// <summary>
    /// Người gán vai trò
    /// </summary>
    public Guid? AssignedBy { get; set; }

    /// <summary>
    /// Navigation properties
    /// </summary>
    public SmartBoxUser? User { get; set; }
    public SmartBoxRole? Role { get; set; }

    protected SmartBoxUserRole() { }

    public SmartBoxUserRole(
        Guid userId,
        Guid roleId,
        Guid? assignedBy = null,
        Guid? tenantId = null
    )
    {
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
        AssignedDate = DateTime.UtcNow;
        TenantId = tenantId;
    }

    public override object[] GetKeys()
    {
        return [UserId, RoleId];
    }
} 