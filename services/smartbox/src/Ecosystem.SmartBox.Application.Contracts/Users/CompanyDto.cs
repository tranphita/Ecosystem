using System;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho thông tin công ty
/// </summary>
public class CompanyDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// Tên công ty
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã số thuế
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Địa chỉ công ty
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Email liên hệ
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Website
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Logo công ty (URL hoặc base64)
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Mô tả công ty
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; }
} 