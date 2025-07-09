using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho người dùng SmartBox
/// </summary>
public class SmartBoxUserDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// ID người dùng từ AuthServer
    /// </summary>
    public Guid AuthUserId { get; set; }

    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Họ tên đầy đủ
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Ngày tháng năm sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Giới tính (0: Không xác định, 1: Nam, 2: Nữ)
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// Avatar URL hoặc base64
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// ID công ty làm việc
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Thông tin công ty
    /// </summary>
    public CompanyDto? Company { get; set; }

    /// <summary>
    /// Chức vụ trong công ty
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Phòng ban
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Mã nhân viên
    /// </summary>
    public string? EmployeeCode { get; set; }

    /// <summary>
    /// Ngày bắt đầu làm việc
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Mức lương
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Lần đăng nhập cuối
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// Danh sách vai trò của user
    /// </summary>
    public List<SmartBoxRoleDto> Roles { get; set; } = new();
} 