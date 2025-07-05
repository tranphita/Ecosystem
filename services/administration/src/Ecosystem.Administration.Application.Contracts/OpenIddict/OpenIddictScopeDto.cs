#nullable enable
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// DTO cho OpenIddict Scope - Phạm vi quyền truy cập
/// </summary>
public class OpenIddictScopeDto : EntityDto<string>
{
    /// <summary>
    /// Tên scope - Định danh duy nhất
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tên hiển thị của scope
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Mô tả scope
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mô tả đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? Descriptions { get; set; }

    /// <summary>
    /// Danh sách tài nguyên mà scope này có thể truy cập
    /// </summary>
    public List<string>? Resources { get; set; }

    /// <summary>
    /// Thời gian tạo
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Thời gian sửa đổi cuối cùng
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}

/// <summary>
/// DTO để tạo mới OpenIddict Scope
/// </summary>
public class CreateOpenIddictScopeDto
{
    /// <summary>
    /// Tên scope - Định danh duy nhất
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tên hiển thị của scope
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Mô tả scope
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mô tả đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? Descriptions { get; set; }

    /// <summary>
    /// Danh sách tài nguyên mà scope này có thể truy cập
    /// </summary>
    public List<string>? Resources { get; set; }
}

/// <summary>
/// DTO để cập nhật OpenIddict Scope
/// </summary>
public class UpdateOpenIddictScopeDto
{
    /// <summary>
    /// Tên hiển thị của scope
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Mô tả scope
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mô tả đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? Descriptions { get; set; }

    /// <summary>
    /// Danh sách tài nguyên mà scope này có thể truy cập
    /// </summary>
    public List<string>? Resources { get; set; }
}

/// <summary>
/// DTO để lấy danh sách OpenIddict Scopes với phân trang
/// </summary>
public class GetOpenIddictScopeListDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Từ khóa tìm kiếm
    /// </summary>
    public string? Filter { get; set; }
} 