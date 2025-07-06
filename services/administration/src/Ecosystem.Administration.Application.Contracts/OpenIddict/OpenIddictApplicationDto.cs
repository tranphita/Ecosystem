#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    [Required(ErrorMessage = "Client ID là bắt buộc")]
    [StringLength(200, ErrorMessage = "Client ID không được vượt quá 200 ký tự")]
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Client Secret - Mật khẩu bí mật của ứng dụng
    /// </summary>
    [StringLength(500, ErrorMessage = "Client Secret không được vượt quá 500 ký tự")]
    [JsonPropertyName("clientSecret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Loại đồng ý (explicit, implicit, external, systematic)
    /// </summary>
    [StringLength(50, ErrorMessage = "Loại đồng ý không được vượt quá 50 ký tự")]
    [JsonPropertyName("consentType")]
    public string? ConsentType { get; set; }

    /// <summary>
    /// Tên hiển thị của ứng dụng
    /// </summary>
    [StringLength(200, ErrorMessage = "Tên hiển thị không được vượt quá 200 ký tự")]
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    [JsonPropertyName("displayNames")]
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Loại ứng dụng (web, native, hybrid)
    /// </summary>
    [StringLength(50, ErrorMessage = "Loại ứng dụng không được vượt quá 50 ký tự")]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// URL của ứng dụng
    /// </summary>
    [StringLength(2000, ErrorMessage = "URL ứng dụng không được vượt quá 2000 ký tự")]
    [JsonPropertyName("clientUri")]
    public string? ClientUri { get; set; }

    /// <summary>
    /// URL logo của ứng dụng
    /// </summary>
    [StringLength(2000, ErrorMessage = "URL logo không được vượt quá 2000 ký tự")]
    [JsonPropertyName("logoUri")]
    public string? LogoUri { get; set; }

    /// <summary>
    /// Danh sách quyền được cấp cho ứng dụng
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng xuất
    /// </summary>
    [JsonPropertyName("postLogoutRedirectUris")]
    public List<string>? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng nhập
    /// </summary>
    [JsonPropertyName("redirectUris")]
    public List<string>? RedirectUris { get; set; }

    /// <summary>
    /// Danh sách yêu cầu bắt buộc
    /// </summary>
    [JsonPropertyName("requirements")]
    public List<string>? Requirements { get; set; }

    /// <summary>
    /// Danh sách loại grant được hỗ trợ
    /// </summary>
    [JsonPropertyName("grantTypes")]
    public List<string>? GrantTypes { get; set; }

    /// <summary>
    /// Danh sách loại response được hỗ trợ
    /// </summary>
    [JsonPropertyName("responseTypes")]
    public List<string>? ResponseTypes { get; set; }

    /// <summary>
    /// Danh sách scope được ứng dụng yêu cầu
    /// </summary>
    [JsonPropertyName("scopes")]
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
    [StringLength(500, ErrorMessage = "Client Secret không được vượt quá 500 ký tự")]
    [JsonPropertyName("clientSecret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Loại đồng ý (explicit, implicit, external, systematic)
    /// </summary>
    [StringLength(50, ErrorMessage = "Loại đồng ý không được vượt quá 50 ký tự")]
    [JsonPropertyName("consentType")]
    public string? ConsentType { get; set; }

    /// <summary>
    /// Tên hiển thị của ứng dụng
    /// </summary>
    [StringLength(200, ErrorMessage = "Tên hiển thị không được vượt quá 200 ký tự")]
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Tên hiển thị đa ngôn ngữ
    /// </summary>
    [JsonPropertyName("displayNames")]
    public Dictionary<string, string>? DisplayNames { get; set; }

    /// <summary>
    /// Loại ứng dụng (web, native, hybrid)
    /// </summary>
    [StringLength(50, ErrorMessage = "Loại ứng dụng không được vượt quá 50 ký tự")]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// URL của ứng dụng
    /// </summary>
    [StringLength(2000, ErrorMessage = "URL ứng dụng không được vượt quá 2000 ký tự")]
    [JsonPropertyName("clientUri")]
    public string? ClientUri { get; set; }

    /// <summary>
    /// URL logo của ứng dụng
    /// </summary>
    [StringLength(2000, ErrorMessage = "URL logo không được vượt quá 2000 ký tự")]
    [JsonPropertyName("logoUri")]
    public string? LogoUri { get; set; }

    /// <summary>
    /// Danh sách quyền được cấp cho ứng dụng
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng xuất
    /// </summary>
    [JsonPropertyName("postLogoutRedirectUris")]
    public List<string>? PostLogoutRedirectUris { get; set; }

    /// <summary>
    /// Danh sách URL chuyển hướng sau khi đăng nhập
    /// </summary>
    [JsonPropertyName("redirectUris")]
    public List<string>? RedirectUris { get; set; }

    /// <summary>
    /// Danh sách yêu cầu bắt buộc
    /// </summary>
    [JsonPropertyName("requirements")]
    public List<string>? Requirements { get; set; }

    /// <summary>
    /// Danh sách loại grant được hỗ trợ
    /// </summary>
    [JsonPropertyName("grantTypes")]
    public List<string>? GrantTypes { get; set; }

    /// <summary>
    /// Danh sách loại response được hỗ trợ
    /// </summary>
    [JsonPropertyName("responseTypes")]
    public List<string>? ResponseTypes { get; set; }

    /// <summary>
    /// Danh sách scope được ứng dụng yêu cầu
    /// </summary>
    [JsonPropertyName("scopes")]
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