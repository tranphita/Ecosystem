namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Hằng số cho OpenIddict - Các giá trị chuẩn theo OAuth 2.0 và OpenID Connect
/// </summary>
public static class OpenIddictConsts
{
    /// <summary>
    /// Loại ứng dụng
    /// </summary>
    public static class ApplicationTypes
    {
        /// <summary>
        /// Ứng dụng web (có khả năng bảo mật client secret)
        /// </summary>
        public const string Web = "web";

        /// <summary>
        /// Ứng dụng native (không thể bảo mật client secret)
        /// </summary>
        public const string Native = "native";

        /// <summary>
        /// Ứng dụng hybrid (kết hợp web và native)
        /// </summary>
        public const string Hybrid = "hybrid";
    }

    /// <summary>
    /// Loại Grant Types
    /// </summary>
    public static class GrantTypes
    {
        /// <summary>
        /// Authorization Code Grant
        /// </summary>
        public const string AuthorizationCode = "authorization_code";

        /// <summary>
        /// Client Credentials Grant
        /// </summary>
        public const string ClientCredentials = "client_credentials";

        /// <summary>
        /// Refresh Token Grant
        /// </summary>
        public const string RefreshToken = "refresh_token";

        /// <summary>
        /// Resource Owner Password Credentials Grant
        /// </summary>
        public const string Password = "password";

        /// <summary>
        /// Implicit Grant
        /// </summary>
        public const string Implicit = "implicit";

        /// <summary>
        /// Device Authorization Grant
        /// </summary>
        public const string DeviceCode = "urn:ietf:params:oauth:grant-type:device_code";
    }

    /// <summary>
    /// Loại Response Types
    /// </summary>
    public static class ResponseTypes
    {
        /// <summary>
        /// Authorization Code Response
        /// </summary>
        public const string Code = "code";

        /// <summary>
        /// Access Token Response
        /// </summary>
        public const string Token = "token";

        /// <summary>
        /// ID Token Response
        /// </summary>
        public const string IdToken = "id_token";

        /// <summary>
        /// Code + ID Token Response
        /// </summary>
        public const string CodeIdToken = "code id_token";

        /// <summary>
        /// Code + Token Response
        /// </summary>
        public const string CodeToken = "code token";

        /// <summary>
        /// ID Token + Token Response
        /// </summary>
        public const string IdTokenToken = "id_token token";

        /// <summary>
        /// Code + ID Token + Token Response
        /// </summary>
        public const string CodeIdTokenToken = "code id_token token";
    }

    /// <summary>
    /// Loại Consent Types
    /// </summary>
    public static class ConsentTypes
    {
        /// <summary>
        /// Yêu cầu sự đồng ý rõ ràng từ người dùng
        /// </summary>
        public const string Explicit = "explicit";

        /// <summary>
        /// Sự đồng ý được xử lý bởi hệ thống bên ngoài
        /// </summary>
        public const string External = "external";

        /// <summary>
        /// Sự đồng ý được cấp ngầm định
        /// </summary>
        public const string Implicit = "implicit";

        /// <summary>
        /// Sự đồng ý được cấp tự động bởi hệ thống
        /// </summary>
        public const string Systematic = "systematic";
    }

    /// <summary>
    /// Scopes chuẩn OpenID Connect
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// Scope cơ bản OpenID Connect
        /// </summary>
        public const string OpenId = "openid";

        /// <summary>
        /// Thông tin profile người dùng
        /// </summary>
        public const string Profile = "profile";

        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public const string Email = "email";

        /// <summary>
        /// Địa chỉ người dùng
        /// </summary>
        public const string Address = "address";

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public const string Phone = "phone";

        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public const string Roles = "roles";

        /// <summary>
        /// Quyền truy cập offline
        /// </summary>
        public const string OfflineAccess = "offline_access";
    }

    /// <summary>
    /// Permissions chuẩn OpenIddict
    /// </summary>
    public static class Permissions
    {
        /// <summary>
        /// Quyền sử dụng Authorization Code endpoint
        /// </summary>
        public const string EndpointAuthorization = "ept:authorization";

        /// <summary>
        /// Quyền sử dụng Logout endpoint
        /// </summary>
        public const string EndpointLogout = "ept:logout";

        /// <summary>
        /// Quyền sử dụng Token endpoint
        /// </summary>
        public const string EndpointToken = "ept:token";

        /// <summary>
        /// Quyền sử dụng Userinfo endpoint
        /// </summary>
        public const string EndpointUserinfo = "ept:userinfo";

        /// <summary>
        /// Quyền sử dụng Introspection endpoint
        /// </summary>
        public const string EndpointIntrospection = "ept:introspection";

        /// <summary>
        /// Quyền sử dụng Revocation endpoint
        /// </summary>
        public const string EndpointRevocation = "ept:revocation";

        /// <summary>
        /// Quyền sử dụng Device endpoint
        /// </summary>
        public const string EndpointDevice = "ept:device";
    }

    /// <summary>
    /// Requirements cho ứng dụng
    /// </summary>
    public static class Requirements
    {
        /// <summary>
        /// Yêu cầu Proof Key for Code Exchange (PKCE)
        /// </summary>
        public const string ProofKeyForCodeExchange = "pkce";
    }
} 