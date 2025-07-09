using System.ComponentModel.DataAnnotations;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// DTO cho tạo và cập nhật công ty
/// </summary>
public class CreateUpdateCompanyDto
{
    /// <summary>
    /// Tên công ty
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã số thuế
    /// </summary>
    [StringLength(50)]
    public string? TaxCode { get; set; }

    /// <summary>
    /// Địa chỉ công ty
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Email liên hệ
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Website
    /// </summary>
    [StringLength(200)]
    [Url]
    public string? Website { get; set; }

    /// <summary>
    /// Logo công ty (URL hoặc base64)
    /// </summary>
    [StringLength(1000)]
    public string? Logo { get; set; }

    /// <summary>
    /// Mô tả công ty
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;
} 