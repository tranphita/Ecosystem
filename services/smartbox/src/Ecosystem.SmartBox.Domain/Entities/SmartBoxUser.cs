using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Ecosystem.SmartBox.Entities;

/// <summary>
/// Entity quản lý người dùng trong SmartBox với thông tin mở rộng
/// </summary>
public class SmartBoxUser : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// ID người dùng từ AuthServer (Identity Service)
    /// Đây là key để liên kết với user authentication
    /// </summary>
    [Required]
    public Guid AuthUserId { get; set; }

    /// <summary>
    /// Tên đăng nhập (sync từ AuthServer)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email (sync từ AuthServer)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Họ tên đầy đủ
    /// </summary>
    [StringLength(200)]
    public string? FullName { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Giới tính (0: Không xác định, 1: Nam, 2: Nữ)
    /// </summary>
    public int Gender { get; set; } = 0;

    /// <summary>
    /// Avatar URL hoặc base64
    /// </summary>
    [StringLength(1000)]
    public string? Avatar { get; set; }

    /// <summary>
    /// ID công ty làm việc
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Chức vụ trong công ty
    /// </summary>
    [StringLength(100)]
    public string? Position { get; set; }

    /// <summary>
    /// Phòng ban
    /// </summary>
    [StringLength(100)]
    public string? Department { get; set; }

    /// <summary>
    /// Mã nhân viên
    /// </summary>
    [StringLength(50)]
    public string? EmployeeCode { get; set; }

    /// <summary>
    /// Ngày bắt đầu làm việc
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Mức lương (nếu cần thiết)
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Địa chỉ
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lần đăng nhập cuối
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// Navigation property đến Company
    /// </summary>
    public Company? Company { get; set; }

    protected SmartBoxUser() { }

    public SmartBoxUser(
        Guid id,
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        Guid? companyId = null,
        Guid? tenantId = null
    ) : base(id)
    {
        AuthUserId = authUserId;
        UserName = userName;
        Email = email;
        FullName = fullName;
        CompanyId = companyId;
        TenantId = tenantId;
    }

    /// <summary>
    /// Cập nhật thông tin đăng nhập cuối
    /// </summary>
    public void UpdateLastLoginTime()
    {
        LastLoginTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật thông tin cơ bản từ AuthServer
    /// </summary>
    public void UpdateBasicInfo(string userName, string email, string? fullName = null)
    {
        UserName = userName;
        Email = email;
        if (!string.IsNullOrEmpty(fullName))
        {
            FullName = fullName;
        }
    }
} 