using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Ecosystem.SmartBox.Entities;

/// <summary>
/// Entity quản lý thông tin công ty
/// </summary>
public class Company : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Tên công ty
    /// </summary>
    [Required]
    [StringLength(200)]
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
    public string? Email { get; set; }

    /// <summary>
    /// Website
    /// </summary>
    [StringLength(200)]
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

    protected Company() { }

    public Company(
        Guid id,
        string name,
        string? taxCode = null,
        string? address = null,
        string? phoneNumber = null,
        string? email = null,
        Guid? tenantId = null
    ) : base(id)
    {
        Name = name;
        TaxCode = taxCode;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        TenantId = tenantId;
    }
} 