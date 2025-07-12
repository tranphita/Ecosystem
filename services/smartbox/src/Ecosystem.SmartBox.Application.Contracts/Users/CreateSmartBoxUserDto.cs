using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho t?o ng??i dùng SmartBox m?i (bao g?m thông tin ??ng nh?p)
/// </summary>
public class CreateSmartBoxUserDto
{
    /// <summary>
    /// Tên ??ng nh?p (b?t bu?c)
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email (b?t bu?c)
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// M?t kh?u (b?t bu?c)
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// H? tên ??y ??
    /// </summary>
    [StringLength(200)]
    public string? FullName { get; set; }

    /// <summary>
    /// S? ?i?n tho?i
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Ngày tháng n?m sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gi?i tính (0: Không xác ??nh, 1: Nam, 2: N?)
    /// </summary>
    [Range(0, 2)]
    public int Gender { get; set; } = 0;

    /// <summary>
    /// Avatar URL ho?c base64
    /// </summary>
    [StringLength(1000)]
    public string? Avatar { get; set; }

    /// <summary>
    /// ID công ty làm vi?c
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Ch?c v? trong công ty
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
    /// Ngày b?t ??u làm vi?c
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// M?c l??ng
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? Salary { get; set; }

    /// <summary>
    /// ??a ch?
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Có yêu c?u ??i m?t kh?u khi ??ng nh?p ??u tiên không
    /// </summary>
    public bool RequirePasswordChange { get; set; } = true;

    /// <summary>
    /// Danh sách ID vai trò gán cho user
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
}