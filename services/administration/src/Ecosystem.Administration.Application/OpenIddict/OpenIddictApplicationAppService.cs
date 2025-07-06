#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ecosystem.Administration.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Application Service cho quản lý OpenIddict Applications
/// </summary>
[Authorize(AdministrationPermissions.OpenIddict.Applications.Default)]
public class OpenIddictApplicationAppService : AdministrationAppService, IOpenIddictApplicationAppService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly ILogger<OpenIddictApplicationAppService> _logger;

    public OpenIddictApplicationAppService(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        ILogger<OpenIddictApplicationAppService> logger)
    {
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách applications với phân trang và tìm kiếm
    /// </summary>
    public async Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationListDto input)
    {
        var applications = new List<object>();
        await foreach (var application in _applicationManager.ListAsync())
        {
            applications.Add(application);
        }

        var items = new List<OpenIddictApplicationDto>();
        foreach (var application in applications.Skip(input.SkipCount).Take(input.MaxResultCount))
        {
            var dto = await MapToApplicationDtoAsync(application);

            // Áp dụng bộ lọc tìm kiếm
            if (!string.IsNullOrEmpty(input.Filter))
            {
                if (!dto.ClientId.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) &&
                    !(dto.DisplayName?.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    continue;
                }
            }

            items.Add(dto);
        }

        return new PagedResultDto<OpenIddictApplicationDto>(
            applications.Count,
            items
        );
    }

    /// <summary>
    /// Lấy thông tin chi tiết một application theo ID
    /// </summary>
    public async Task<OpenIddictApplicationDto> GetAsync(string id)
    {
        var application = await _applicationManager.FindByIdAsync(id);
        if (application == null)
        {
            throw new Volo.Abp.UserFriendlyException("Application not found");
        }

        return await MapToApplicationDtoAsync(application);
    }

    /// <summary>
    /// Tạo mới OpenIddict Application với validation đầy đủ và tuân thủ OpenID Connect specifications
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Applications.Create)]
    public async Task<OpenIddictApplicationDto> CreateAsync(CreateOpenIddictApplicationDto input)
    {
        using var activity = Logger.BeginScope("Tạo OpenIddict Application với ClientId: {ClientId}", input.ClientId);
        
        try
        {
            // Bước 1: Validation đầu vào cơ bản
            await ValidateCreateInputAsync(input);

            // Bước 2: Kiểm tra Client ID đã tồn tại
            await EnsureClientIdNotExistsAsync(input.ClientId);

            // Bước 3: Tạo và cấu hình descriptor
            var descriptor = await CreateApplicationDescriptorAsync(input);

            // Bước 4: Validate business rules trước khi tạo
            await ValidateBusinessRulesAsync(descriptor, input);
            
            var application = await _applicationManager.CreateAsync(descriptor);
            var applicationId = await _applicationManager.GetIdAsync(application);
            return await MapToApplicationDtoAsync(application);
        }
        catch (OpenIddictExceptions.ValidationException ex)
        {
            throw new Volo.Abp.UserFriendlyException($"Lỗi validation OpenIddict: {ex.Message}");
        }
        catch
        {
            throw new Volo.Abp.UserFriendlyException("Đã xảy ra lỗi khi tạo ứng dụng. Vui lòng thử lại.");
        }
    }

    /// <summary>
    /// Validate đầu vào cơ bản cho việc tạo application
    /// </summary>
    private Task ValidateCreateInputAsync(CreateOpenIddictApplicationDto input)
    {
        // Validate Client ID format và ký tự đặc biệt
        if (input.ClientId.Contains(" ") || input.ClientId.Contains("\t") || input.ClientId.Contains("\n"))
        {
            throw new Volo.Abp.UserFriendlyException("Client ID không được chứa khoảng trắng hoặc ký tự xuống dòng");
        }

        // Validate Client Type
        if (!string.IsNullOrEmpty(input.Type))
        {
            var validTypes = new[] { 
                OpenIddictConstants.ClientTypes.Confidential, 
                OpenIddictConstants.ClientTypes.Public 
            };
            
            if (!validTypes.Contains(input.Type))
            {
                throw new Volo.Abp.UserFriendlyException($"Loại client không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validTypes)}");
            }
        }

        // Validate Consent Type
        if (!string.IsNullOrEmpty(input.ConsentType))
        {
            var validConsentTypes = new[] { 
                OpenIddictConsts.ConsentTypes.Explicit,
                OpenIddictConsts.ConsentTypes.External,
                OpenIddictConsts.ConsentTypes.Implicit,
                OpenIddictConsts.ConsentTypes.Systematic
            };
            
            if (!validConsentTypes.Contains(input.ConsentType))
            {
                throw new Volo.Abp.UserFriendlyException($"Loại consent không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validConsentTypes)}");
            }
        }

        // Validate Grant Types và Response Types compatibility
        ValidateGrantAndResponseTypesCompatibility(input.GrantTypes, input.ResponseTypes);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Kiểm tra tính tương thích giữa Grant Types và Response Types theo OpenID Connect spec
    /// </summary>
    private void ValidateGrantAndResponseTypesCompatibility(List<string>? grantTypes, List<string>? responseTypes)
    {
        if (grantTypes == null || !grantTypes.Any() || responseTypes == null || !responseTypes.Any())
        {
            return;
        }

        // Authorization Code flow phải có code response type
        if (grantTypes.Contains(OpenIddictConsts.GrantTypes.AuthorizationCode) &&
            !responseTypes.Contains(OpenIddictConsts.ResponseTypes.Code))
        {
            throw new Volo.Abp.UserFriendlyException(
                "Authorization Code Grant yêu cầu Response Type 'code'");
        }

        // Implicit flow phải có token hoặc id_token response type
        if (grantTypes.Contains(OpenIddictConsts.GrantTypes.Implicit))
        {
            var hasValidImplicitResponse = responseTypes.Any(rt => 
                rt == OpenIddictConsts.ResponseTypes.Token ||
                rt == OpenIddictConsts.ResponseTypes.IdToken ||
                rt == OpenIddictConsts.ResponseTypes.IdTokenToken);
                
            if (!hasValidImplicitResponse)
            {
                throw new Volo.Abp.UserFriendlyException(
                    "Implicit Grant yêu cầu Response Type 'token', 'id_token' hoặc 'id_token token'");
            }
        }

        // Client Credentials flow không được có response types
        if (grantTypes.Contains(OpenIddictConsts.GrantTypes.ClientCredentials) && responseTypes.Any())
        {
            throw new Volo.Abp.UserFriendlyException(
                "Client Credentials Grant không được có Response Types");
        }
    }

    /// <summary>
    /// Đảm bảo Client ID chưa tồn tại trong hệ thống
    /// </summary>
    private async Task EnsureClientIdNotExistsAsync(string clientId)
    {
        var existingApplication = await _applicationManager.FindByClientIdAsync(clientId);
        if (existingApplication != null)
        {
            throw new Volo.Abp.UserFriendlyException($"Client ID '{clientId}' đã tồn tại trong hệ thống");
        }
    }

    /// <summary>
    /// Tạo và cấu hình OpenIddictApplicationDescriptor từ input data
    /// </summary>
    private async Task<OpenIddictApplicationDescriptor> CreateApplicationDescriptorAsync(CreateOpenIddictApplicationDto input)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = input.ClientId.Trim(),
            ConsentType = input.ConsentType ?? OpenIddictConsts.ConsentTypes.Explicit,
            DisplayName = input.DisplayName?.Trim(),
            ApplicationType = input.Type ?? OpenIddictConstants.ClientTypes.Confidential
        };

        // Cấu hình Client Secret với validation bảo mật
        ConfigureClientSecret(descriptor, input);

        // Cấu hình URIs với validation format
        ConfigureApplicationUris(descriptor, input);

        // Cấu hình DisplayNames đa ngôn ngữ
        ConfigureDisplayNames(descriptor, input);

        // Cấu hình Permissions với validation
        await ConfigurePermissionsAsync(descriptor, input);

        // Cấu hình Redirect URIs với validation security
        ConfigureRedirectUris(descriptor, input);

        // Cấu hình Post Logout Redirect URIs
        ConfigurePostLogoutRedirectUris(descriptor, input);

        // Cấu hình Requirements (PKCE, etc.)
        ConfigureRequirements(descriptor, input);

        return descriptor;
    }

    /// <summary>
    /// Cấu hình Client Secret với validation bảo mật
    /// </summary>
    private void ConfigureClientSecret(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        if (!string.IsNullOrEmpty(input.ClientSecret))
        {
            // Kiểm tra Client Type compatibility
            if (descriptor.ApplicationType == OpenIddictConstants.ClientTypes.Public)
            {
                throw new Volo.Abp.UserFriendlyException(
                    "Public client không được có Client Secret vì lý do bảo mật");
            }

            // Validate Client Secret strength (tối thiểu 16 ký tự)
            if (input.ClientSecret.Length < 16)
            {
                throw new Volo.Abp.UserFriendlyException(
                    "Client Secret phải có ít nhất 16 ký tự để đảm bảo bảo mật");
            }

            descriptor.ClientSecret = input.ClientSecret;
        }
    }

    /// <summary>
    /// Cấu hình Application URIs với validation format
    /// </summary>
    private void ConfigureApplicationUris(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        // Validate Client URI
        if (!string.IsNullOrEmpty(input.ClientUri))
        {
            if (!Uri.TryCreate(input.ClientUri, UriKind.Absolute, out var clientUri) || 
                (!clientUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) && 
                 !clientUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
            {
                throw new Volo.Abp.UserFriendlyException($"Client URI không hợp lệ: {input.ClientUri}");
            }
        }

        // Validate Logo URI
        if (!string.IsNullOrEmpty(input.LogoUri))
        {
            if (!Uri.TryCreate(input.LogoUri, UriKind.Absolute, out var logoUri))
            {
                throw new Volo.Abp.UserFriendlyException($"Logo URI không hợp lệ: {input.LogoUri}");
            }
        }
    }

    /// <summary>
    /// Cấu hình DisplayNames đa ngôn ngữ
    /// </summary>
    private void ConfigureDisplayNames(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        if (input.DisplayNames != null && input.DisplayNames.Any())
        {
            foreach (var displayName in input.DisplayNames)
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(displayName.Key);
                    descriptor.DisplayNames.Add(culture, displayName.Value?.Trim() ?? string.Empty);
                }
                catch (CultureNotFoundException)
                {
                    _logger.LogWarning("Culture không hợp lệ được bỏ qua: {Culture}", displayName.Key);
                }
            }
        }
    }

    /// <summary>
    /// Cấu hình Permissions với validation
    /// </summary>
    private async Task ConfigurePermissionsAsync(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        // Thêm permissions cơ bản
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);

        // Cấu hình permissions dựa trên Grant Types
        if (input.GrantTypes != null && input.GrantTypes.Any())
        {
            await ConfigureGrantTypePermissionsAsync(descriptor, input.GrantTypes);
        }

        // Cấu hình permissions dựa trên Response Types
        if (input.ResponseTypes != null && input.ResponseTypes.Any())
        {
            ConfigureResponseTypePermissions(descriptor, input.ResponseTypes);
        }

        // Cấu hình Scope permissions
        if (input.Scopes != null && input.Scopes.Any())
        {
            await ConfigureScopePermissionsAsync(descriptor, input.Scopes);
        }

        // Thêm custom permissions từ input
        if (input.Permissions != null && input.Permissions.Any())
        {
            foreach (var permission in input.Permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var trimmedPermission = permission.Trim();
                if (!descriptor.Permissions.Contains(trimmedPermission))
                {
                    descriptor.Permissions.Add(trimmedPermission);
                }
            }
        }
    }

    /// <summary>
    /// Cấu hình permissions cho Grant Types
    /// </summary>
    private Task ConfigureGrantTypePermissionsAsync(OpenIddictApplicationDescriptor descriptor, List<string> grantTypes)
    {
        foreach (var grantType in grantTypes.Where(gt => !string.IsNullOrWhiteSpace(gt)))
        {
            var trimmedGrantType = grantType.Trim();
            
            switch (trimmedGrantType)
            {
                case OpenIddictConstants.GrantTypes.AuthorizationCode:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    break;
                    
                case OpenIddictConstants.GrantTypes.ClientCredentials:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    break;
                    
                case OpenIddictConstants.GrantTypes.RefreshToken:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    break;
                    
                case OpenIddictConstants.GrantTypes.Implicit:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    break;
                    
                case OpenIddictConstants.GrantTypes.Password:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    break;
            }
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cấu hình permissions cho Response Types
    /// </summary>
    private void ConfigureResponseTypePermissions(OpenIddictApplicationDescriptor descriptor, List<string> responseTypes)
    {
        foreach (var responseType in responseTypes.Where(rt => !string.IsNullOrWhiteSpace(rt)))
        {
            var trimmedResponseType = responseType.Trim();
            
            switch (trimmedResponseType)
            {
                case OpenIddictConstants.ResponseTypes.Code:
                    if (!descriptor.Permissions.Contains(OpenIddictConstants.Permissions.ResponseTypes.Code))
                    {
                        descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    }
                    break;
                    
                case OpenIddictConstants.ResponseTypes.Token:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                    break;
                    
                case OpenIddictConstants.ResponseTypes.IdToken:
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                    break;
                    
                case "code id_token":
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);
                    break;
                    
                case "code token":
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
                    break;
                    
                case "id_token token":
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                    break;
                    
                case "code id_token token":
                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                    break;
            }
        }
    }

    /// <summary>
    /// Cấu hình permissions cho Scopes
    /// </summary>
    private async Task ConfigureScopePermissionsAsync(OpenIddictApplicationDescriptor descriptor, List<string> scopes)
    {
        var availableScopes = await GetAvailableScopesAsync();
        
        foreach (var scope in scopes.Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            var trimmedScope = scope.Trim();
            
            // Scopes chuẩn
            var standardScopes = new[]
            {
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "offline_access"
            };
            
            if (standardScopes.Contains(trimmedScope))
            {
                descriptor.Permissions.Add(trimmedScope);
            }
            else if (availableScopes.Contains(trimmedScope))
            {
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + trimmedScope);
            }
            else
            {
                _logger.LogWarning("Scope không hợp lệ được bỏ qua: {Scope}", scope);
            }
        }
    }

    /// <summary>
    /// Cấu hình Redirect URIs với validation bảo mật
    /// </summary>
    private void ConfigureRedirectUris(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        if (input.RedirectUris != null && input.RedirectUris.Any())
        {
            foreach (var uriString in input.RedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                var trimmedUri = uriString.Trim();
                
                if (!Uri.TryCreate(trimmedUri, UriKind.Absolute, out var uri))
                {
                    throw new Volo.Abp.UserFriendlyException($"Redirect URI không hợp lệ: {trimmedUri}");
                }

                ValidateRedirectUriSecurity(uri, input.Type);
                descriptor.RedirectUris.Add(uri);
            }
        }
    }

    /// <summary>
    /// Validate bảo mật cho Redirect URIs
    /// </summary>
    private void ValidateRedirectUriSecurity(Uri uri, string? clientType)
    {
        // Public clients không được sử dụng HTTP (trừ localhost cho development)
        if (clientType == OpenIddictConstants.ClientTypes.Public && 
            uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
            !uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) &&
            !uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            throw new Volo.Abp.UserFriendlyException(
                $"Public client không được sử dụng HTTP redirect URI (trừ localhost): {uri}");
        }

        // Không cho phép wildcard trong redirect URIs
        if (uri.ToString().Contains("*"))
        {
            throw new Volo.Abp.UserFriendlyException(
                $"Redirect URI không được chứa wildcard: {uri}");
        }

        // Không cho phép fragment trong redirect URIs
        if (!string.IsNullOrEmpty(uri.Fragment))
        {
            throw new Volo.Abp.UserFriendlyException(
                $"Redirect URI không được chứa fragment: {uri}");
        }
    }

    /// <summary>
    /// Cấu hình Post Logout Redirect URIs
    /// </summary>
    private void ConfigurePostLogoutRedirectUris(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        if (input.PostLogoutRedirectUris != null && input.PostLogoutRedirectUris.Any())
        {
            foreach (var uriString in input.PostLogoutRedirectUris.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                var trimmedUri = uriString.Trim();
                
                if (!Uri.TryCreate(trimmedUri, UriKind.Absolute, out var uri))
                {
                    throw new Volo.Abp.UserFriendlyException($"Post Logout Redirect URI không hợp lệ: {trimmedUri}");
                }

                descriptor.PostLogoutRedirectUris.Add(uri);
            }
        }
    }

    /// <summary>
    /// Cấu hình Requirements (PKCE, etc.)
    /// </summary>
    private void ConfigureRequirements(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        if (input.Requirements != null && input.Requirements.Any())
        {
            foreach (var requirement in input.Requirements.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                var trimmedRequirement = requirement.Trim();
                descriptor.Requirements.Add(trimmedRequirement);
            }
        }
    }

    /// <summary>
    /// Cập nhật thông tin application
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Applications.Update)]
    public async Task<OpenIddictApplicationDto> UpdateAsync(string id, UpdateOpenIddictApplicationDto input)
    {
        try
        {
            var application = await _applicationManager.FindByIdAsync(id);
            if (application == null)
            {
                throw new Volo.Abp.UserFriendlyException("Application not found");
            }

            var descriptor = new OpenIddictApplicationDescriptor();
            await _applicationManager.PopulateAsync(descriptor, application);

            // Cập nhật các thuộc tính
            if (!string.IsNullOrEmpty(input.ClientSecret))
            {
                descriptor.ClientSecret = input.ClientSecret;
            }

            if (!string.IsNullOrEmpty(input.ConsentType))
            {
                descriptor.ConsentType = input.ConsentType;
            }

            if (!string.IsNullOrEmpty(input.DisplayName))
            {
                descriptor.DisplayName = input.DisplayName;
            }

            if (!string.IsNullOrEmpty(input.Type))
            {
                descriptor.ApplicationType = input.Type;
            }

            // Cập nhật DisplayNames
            if (input.DisplayNames != null)
            {
                descriptor.DisplayNames.Clear();
                foreach (var displayName in input.DisplayNames)
                {
                    descriptor.DisplayNames.Add(CultureInfo.GetCultureInfo(displayName.Key), displayName.Value);
                }
            }

            // Cập nhật Permissions
            if (input.Permissions != null)
            {
                descriptor.Permissions.Clear();
                foreach (var permission in input.Permissions)
                {
                    descriptor.Permissions.Add(permission);
                }
            }

            // Cập nhật Redirect URIs với validation
            if (input.RedirectUris != null)
            {
                descriptor.RedirectUris.Clear();
                foreach (var uriString in input.RedirectUris)
                {
                    if (string.IsNullOrWhiteSpace(uriString))
                    {
                        continue;
                    }

                    if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                    {
                        descriptor.RedirectUris.Add(uri);
                    }
                    else
                    {
                        throw new Volo.Abp.UserFriendlyException($"Invalid redirect URI: {uriString}");
                    }
                }
            }

            // Cập nhật Post Logout Redirect URIs với validation
            if (input.PostLogoutRedirectUris != null)
            {
                descriptor.PostLogoutRedirectUris.Clear();
                foreach (var uriString in input.PostLogoutRedirectUris)
                {
                    if (string.IsNullOrWhiteSpace(uriString))
                    {
                        continue;
                    }

                    if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                    {
                        descriptor.PostLogoutRedirectUris.Add(uri);
                    }
                    else
                    {
                        throw new Volo.Abp.UserFriendlyException($"Invalid post logout redirect URI: {uriString}");
                    }
                }
            }

            // Cập nhật Requirements
            if (input.Requirements != null)
            {
                descriptor.Requirements.Clear();
                foreach (var requirement in input.Requirements)
                {
                    descriptor.Requirements.Add(requirement);
                }
            }

            await _applicationManager.UpdateAsync(application, descriptor);
            return await MapToApplicationDtoAsync(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application with id: {Id}, input: {@Input}", id, input);
            throw;
        }
    }

    /// <summary>
    /// Xóa application theo ID
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Applications.Delete)]
    public async Task DeleteAsync(string id)
    {
        var application = await _applicationManager.FindByIdAsync(id);
        if (application == null)
        {
            throw new Volo.Abp.UserFriendlyException("Application not found");
        }

        await _applicationManager.DeleteAsync(application);
    }

    /// <summary>
    /// Kiểm tra Client ID đã tồn tại hay chưa
    /// </summary>
    public async Task<bool> IsClientIdExistsAsync(string clientId)
    {
        var application = await _applicationManager.FindByClientIdAsync(clientId);
        return application != null;
    }

    /// <summary>
    /// Lấy danh sách permissions có sẵn cho applications
    /// </summary>
    public Task<string[]> GetAvailablePermissionsAsync()
    {
        // Trả về danh sách permissions chuẩn của OpenIddict
        return Task.FromResult(new[]
        {
            OpenIddictConsts.Permissions.EndpointAuthorization,
            OpenIddictConsts.Permissions.EndpointLogout,
            OpenIddictConsts.Permissions.EndpointToken,
            OpenIddictConsts.Permissions.EndpointUserinfo,
            OpenIddictConsts.Permissions.EndpointIntrospection,
            OpenIddictConsts.Permissions.EndpointRevocation,
            OpenIddictConsts.Permissions.EndpointDevice
        });
    }

    /// <summary>
    /// Lấy danh sách scopes có sẵn cho applications
    /// </summary>
    public async Task<string[]> GetAvailableScopesAsync()
    {
        var scopes = new List<string>();

        // Thêm scopes chuẩn
        scopes.AddRange(new[]
        {
            OpenIddictConsts.Scopes.OpenId,
            OpenIddictConsts.Scopes.Profile,
            OpenIddictConsts.Scopes.Email,
            OpenIddictConsts.Scopes.Address,
            OpenIddictConsts.Scopes.Phone,
            OpenIddictConsts.Scopes.Roles,
            OpenIddictConsts.Scopes.OfflineAccess
        });

        // Thêm scopes tùy chỉnh từ database
        await foreach (var scope in _scopeManager.ListAsync())
        {
            var scopeName = await _scopeManager.GetNameAsync(scope);
            if (!string.IsNullOrEmpty(scopeName) && !scopes.Contains(scopeName))
            {
                scopes.Add(scopeName);
            }
        }

        return scopes.ToArray();
    }

    /// <summary>
    /// Ánh xạ từ OpenIddict Application entity sang DTO
    /// </summary>
    private async Task<OpenIddictApplicationDto> MapToApplicationDtoAsync(object application)
    {
        var dto = new OpenIddictApplicationDto
        {
            Id = await _applicationManager.GetIdAsync(application) ?? string.Empty,
            ClientId = await _applicationManager.GetClientIdAsync(application) ?? string.Empty,
            ClientSecret = null, // Client secret không được expose vì lý do bảo mật
            ConsentType = await _applicationManager.GetConsentTypeAsync(application),
            DisplayName = await _applicationManager.GetDisplayNameAsync(application),
            Type = await _applicationManager.GetApplicationTypeAsync(application)
        };

        // Ánh xạ DisplayNames
        var displayNames = await _applicationManager.GetDisplayNamesAsync(application);
        if (displayNames.Any())
        {
            dto.DisplayNames = displayNames.ToDictionary(x => x.Key.Name, x => x.Value);
        }

        // Ánh xạ Permissions
        var permissions = await _applicationManager.GetPermissionsAsync(application);
        if (permissions.Any())
        {
            dto.Permissions = permissions.ToList();
        }

        // Ánh xạ Redirect URIs
        var redirectUris = await _applicationManager.GetRedirectUrisAsync(application);
        if (redirectUris.Any())
        {
            dto.RedirectUris = redirectUris.Select(uri => uri.ToString()).ToList();
        }

        // Ánh xạ Post Logout Redirect URIs
        var postLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(application);
        if (postLogoutRedirectUris.Any())
        {
            dto.PostLogoutRedirectUris = postLogoutRedirectUris.Select(uri => uri.ToString()).ToList();
        }

        // Ánh xạ Requirements
        var requirements = await _applicationManager.GetRequirementsAsync(application);
        if (requirements.Any())
        {
            dto.Requirements = requirements.ToList();
        }

        if (application is Volo.Abp.Auditing.IHasCreationTime creationTimeEntity)
        {
            dto.CreationTime = creationTimeEntity.CreationTime;
        }

        if (application is Volo.Abp.Auditing.IHasModificationTime modificationTimeEntity)
        {
            dto.LastModificationTime = modificationTimeEntity.LastModificationTime;
        }

        return dto;
    }

    /// <summary>
    /// Validate business rules trước khi tạo application
    /// </summary>
    private Task ValidateBusinessRulesAsync(OpenIddictApplicationDescriptor descriptor, CreateOpenIddictApplicationDto input)
    {
        // Validate rằng confidential clients có ít nhất một secure flow
        if (descriptor.ApplicationType == OpenIddictConstants.ClientTypes.Confidential)
        {
            var hasSecureFlow = input.GrantTypes?.Any(g => 
                g == OpenIddictConsts.GrantTypes.AuthorizationCode ||
                g == OpenIddictConsts.GrantTypes.ClientCredentials) ?? false;

            if (!hasSecureFlow)
            {
                throw new Volo.Abp.UserFriendlyException(
                    "Confidential client phải có ít nhất một secure flow (Authorization Code hoặc Client Credentials)");
            }
        }

        // Validate rằng public clients không sử dụng các flows không an toàn
        if (descriptor.ApplicationType == OpenIddictConstants.ClientTypes.Public)
        {
            if (input.GrantTypes?.Contains(OpenIddictConsts.GrantTypes.ClientCredentials) == true)
            {
                throw new Volo.Abp.UserFriendlyException(
                    "Public client không được sử dụng Client Credentials flow");
            }
        }

        return Task.CompletedTask;
    }
}