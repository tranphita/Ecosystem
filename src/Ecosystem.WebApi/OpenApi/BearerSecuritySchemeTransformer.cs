using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Ecosystem.WebApi.OpenApi;

/// <summary>
/// Một Document Transformer để tự động thêm hỗ trợ xác thực "Bearer" (JWT)
/// vào tài liệu OpenAPI.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    /// <summary>
    /// Phương thức này sẽ được .NET gọi tự động khi tạo tài liệu OpenAPI.
    /// </summary>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra xem ứng dụng có thực sự đăng ký scheme xác thực "Bearer" hay không.
        // Điều này giúp transformer an toàn, nó sẽ không làm gì nếu ứng dụng không dùng JWT.
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (!authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            return; // Nếu không có scheme "Bearer", không làm gì cả.
        }

        // 2. Tạo định nghĩa cho Security Scheme (cái nút "Authorize" và popup của nó)
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", // Phải là chữ thường
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Description = "Please enter into field the word 'Bearer' followed by a space and the JWT"
        };

        // 3. Thêm Security Scheme vào phần "Components" của tài liệu OpenAPI
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = securityScheme;

        // 4. Tạo yêu cầu bảo mật (biểu tượng ổ khóa trên mỗi endpoint)
        // Yêu cầu này tham chiếu đến Security Scheme "Bearer" đã được định nghĩa ở trên.
        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        };

        // 5. Áp dụng yêu cầu bảo mật này cho TẤT CẢ các operation (endpoint)
        foreach (var path in document.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                operation.Security.Add(securityRequirement);
            }
        }
    }
}