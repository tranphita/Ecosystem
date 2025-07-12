using System;
using Volo.Abp.EventBus;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Integration Event khi user ???c t?o trong SmartBox
/// </summary>
[EventName("SmartBox.User.Created")]
public class UserCreatedIntegrationEvent
{
    /// <summary>
    /// ID c?a user trong SmartBox
    /// </summary>
    public Guid SmartBoxUserId { get; set; }

    /// <summary>
    /// ID c?a user trong Identity Service (n?u ?ã có)
    /// </summary>
    public Guid? AuthUserId { get; set; }

    /// <summary>
    /// Tên ??ng nh?p
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// H? tên ??y ??
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// S? ?i?n tho?i
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// M?t kh?u t?m th?i (s? ???c mã hóa)
    /// </summary>
    public string? TemporaryPassword { get; set; }

    /// <summary>
    /// Có yêu c?u ??i m?t kh?u khi ??ng nh?p ??u tiên không
    /// </summary>
    public bool RequirePasswordChange { get; set; } = true;

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Tenant ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Th?i gian t?o
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Event khi user ???c c?p nh?t trong SmartBox
/// </summary>
[EventName("SmartBox.User.Updated")]
public class UserUpdatedIntegrationEvent
{
    /// <summary>
    /// ID c?a user trong SmartBox
    /// </summary>
    public Guid SmartBoxUserId { get; set; }

    /// <summary>
    /// ID c?a user trong Identity Service
    /// </summary>
    public Guid AuthUserId { get; set; }

    /// <summary>
    /// Tên ??ng nh?p
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// H? tên ??y ??
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// S? ?i?n tho?i
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Th?i gian c?p nh?t
    /// </summary>
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Event khi user ???c xóa trong SmartBox
/// </summary>
[EventName("SmartBox.User.Deleted")]
public class UserDeletedIntegrationEvent
{
    /// <summary>
    /// ID c?a user trong SmartBox
    /// </summary>
    public Guid SmartBoxUserId { get; set; }

    /// <summary>
    /// ID c?a user trong Identity Service
    /// </summary>
    public Guid AuthUserId { get; set; }

    /// <summary>
    /// Tên ??ng nh?p
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Th?i gian xóa
    /// </summary>
    public DateTime DeletionTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Integration Event khi user trong Identity Service ???c t?o/c?p nh?t
/// </summary>
[EventName("Identity.User.Synced")]
public class IdentityUserSyncedIntegrationEvent
{
    /// <summary>
    /// ID c?a user trong Identity Service
    /// </summary>
    public Guid AuthUserId { get; set; }

    /// <summary>
    /// Tên ??ng nh?p
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// H? tên ??y ??
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// S? ?i?n tho?i
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Lo?i thao tác (Created, Updated)
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// Th?i gian ??ng b?
    /// </summary>
    public DateTime SyncTime { get; set; } = DateTime.UtcNow;
}