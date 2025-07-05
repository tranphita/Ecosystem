#nullable enable
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// DTO cho OpenIddict Application - Thông tin ứng dụng OAuth/OpenID Connect
/// </summary>
public class OpenIddictApplicationDto : EntityDto<string>
{
    /// <summary>
    /// Client ID - Định danh duy nhất của ứng dụng
    /// </summary>
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Client Secret - Mật khẩu bí mật của ứng dụng
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Loại đồng ý (explicit, implicit, external, systematic)
    /// </summary>
    public string? ConsentType { get; set; }

    /// <summary>
    /// Tên hiển thị của ứng dụng
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Loại ứng dụng (web, native, hybrid)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// URL của ứng dụng
    /// </summary>
    public string? ClientUri { get; set; }

    /// <summary>
    /// URL logo của ứng dụng
    /// </summary>
    public string? LogoUri { get; set; }

    /// <summary>
    /// Danh sách quyền được cấp cho ứng dụng
    /// </summary>
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng xuất
    /// </summary>
    public List<string>? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng nhập
    /// </summary>
    public List<string>? RedirectUris { get; set; }

    /// <summary>
    /// Danh sách yêu cầu bắt buộc
    /// </summary>
    public List<string>? Requirements { get; set; }

    /// <summary>
    /// Danh sách loại grant được hỗ trợ
    /// </summary>
    public List<string>? GrantTypes { get; set; }

    /// <summary>
    /// Danh sách loại response được hỗ trợ
    /// </summary>
    public List<string>? ResponseTypes { get; set; }

    /// <summary>
    /// Danh sách scope được ứng dụng yêu cầu
    /// </summary>
    public List<string>? Scopes { get; set; }

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
/// DTO để tạo mới OpenIddict Application
/// </summary>
public class CreateOpenIddictApplicationDto
{
    /// <summary>
    /// Client ID - Định danh duy nhất của ứng dụng
    /// </summary>
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Client Secret - Mật khẩu bí mật của ứng dụng
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Loại đồng ý (explicit, implicit, external, systematic)
    /// </summary>
    public string? ConsentType { get; set; }

    /// <summary>
    /// Tên hiển thị của ứng dụng
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Loại ứng dụng (web, native, hybrid)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// URL của ứng dụng
    /// </summary>
    public string? ClientUri { get; set; }

    /// <summary>
    /// URL logo của ứng dụng
    /// </summary>
    public string? LogoUri { get; set; }

    /// <summary>
    /// Danh sách quyền được cấp cho ứng dụng
    /// </summary>
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng xuất
    /// </summary>
    public List<string>? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng nhập
    /// </summary>
    public List<string>? RedirectUris { get; set; }

    /// <summary>
    /// Danh sách yêu cầu bắt buộc
    /// </summary>
    public List<string>? Requirements { get; set; }

    /// <summary>
    /// Danh sách loại grant được hỗ trợ
    /// </summary>
    public List<string>? GrantTypes { get; set; }

    /// <summary>
    /// Danh sách loại response được hỗ trợ
    /// </summary>
    public List<string>? ResponseTypes { get; set; }

    /// <summary>
    /// Danh sách scope được ứng dụng yêu cầu
    /// </summary>
    public List<string>? Scopes { get; set; }
}

/// <summary>
/// DTO để cập nhật OpenIddict Application
/// </summary>
public class UpdateOpenIddictApplicationDto
{
    /// <summary>
    /// Client Secret - Mật khẩu bí mật của ứng dụng
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Loại đồng ý (explicit, implicit, external, systematic)
    /// </summary>
    public string? ConsentType { get; set; }

    /// <summary>
    /// Tên hiển thị của ứng dụng
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Loại ứng dụng (web, native, hybrid)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// URL của ứng dụng
    /// </summary>
    public string? ClientUri { get; set; }

    /// <summary>
    /// URL logo của ứng dụng
    /// </summary>
    public string? LogoUri { get; set; }

    /// <summary>
    /// Danh sách quyền được cấp cho ứng dụng
    /// </summary>
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng xuất
    /// </summary>
    public List<string>? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng nhập
    /// </summary>
    public List<string>? RedirectUris { get; set; }

    /// <summary>
    /// Danh sách yêu cầu bắt buộc
    /// </summary>
    public List<string>? Requirements { get; set; }

    /// <summary>
    /// Danh sách loại grant được hỗ trợ
    /// </summary>
    public List<string>? GrantTypes { get; set; }

    /// <summary>
    /// Danh sách loại response được hỗ trợ
    /// </summary>
    public List<string>? ResponseTypes { get; set; }

    /// <summary>
    /// Danh sách scope được ứng dụng yêu cầu
    /// </summary>
    public List<string>? Scopes { get; set; }
}

/// <summary>
/// DTO để lấy danh sách OpenIddict Applications với phân trang
/// </summary>
public class GetOpenIddictApplicationListDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Từ khóa tìm kiếm
    /// </summary>
    public string? Filter { get; set; }
} 