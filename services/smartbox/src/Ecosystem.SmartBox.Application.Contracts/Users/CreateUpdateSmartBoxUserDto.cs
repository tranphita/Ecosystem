using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho tạo và cập nhật người dùng SmartBox
/// </summary>
public class CreateUpdateSmartBoxUserDto
{
    /// <summary>
    /// Họ tên đầy đủ
    /// </summary>
    [StringLength(200)]
    public string? FullName { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Giới tính (0: Không xác định, 1: Nam, 2: Nữ)
    /// </summary>
    [Range(0, 2)]
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
    /// Mức lương
    /// </summary>
    [Range(0, double.MaxValue)]
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
    /// Danh sách ID vai trò gán cho user
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
} 