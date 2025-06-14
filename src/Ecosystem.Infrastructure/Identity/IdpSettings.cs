namespace Ecosystem.Infrastructure.Identity;

/// <summary>
/// Cấu hình kết nối tới hệ thống xác thực (Identity Provider).
/// </summary>
public class IdpSettings
{
    public const string SectionName = "IdpSettings";
    
    /// <summary>
    /// Địa chỉ gốc của máy chủ xác thực (IDP).
    /// </summary>
    public string BaseUrl { get; init; } = null!;
    
    /// <summary>
    /// Client ID dùng cho xác thực ROPC.
    /// </summary>
    public string ClientId { get; init; } = null!;
    
    /// <summary>
    /// Client secret dùng cho xác thực ROPC.
    /// </summary>
    public string ClientSecret { get; init; } = null!;
    
    /// <summary>
    /// Scope yêu cầu khi lấy token.
    /// </summary>
    public string Scope { get; init; } = null!;
    
    /// <summary>
    /// Endpoint để lấy thông tin người dùng.
    /// </summary>
    public string UserInfoEndpoint { get; init; } = null!;
}
