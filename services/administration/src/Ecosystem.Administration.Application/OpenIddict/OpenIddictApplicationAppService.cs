using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ecosystem.Administration.OpenIddict;
using Ecosystem.Administration.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Application Service cho quản lý OpenIddict Applications
/// </summary>
[Authorize(AdministrationPermissions.OpenIddict.Applications.Default)]
public class OpenIddictApplicationAppService : AdministrationAppService, IOpenIddictApplicationAppService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public OpenIddictApplicationAppService(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
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
    /// Tạo mới một application
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Applications.Create)]
    public async Task<OpenIddictApplicationDto> CreateAsync(CreateOpenIddictApplicationDto input)
    {
        // Kiểm tra Client ID đã tồn tại
        if (await IsClientIdExistsAsync(input.ClientId))
        {
            throw new Volo.Abp.UserFriendlyException("Client ID already exists");
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = input.ClientId,
            ClientSecret = input.ClientSecret,
            ConsentType = input.ConsentType,
            DisplayName = input.DisplayName,
            ClientType = input.Type // Sử dụng ClientType thay vì Type
        };

        // Thêm DisplayNames (đa ngôn ngữ)
        if (input.DisplayNames != null)
        {
            foreach (var displayName in input.DisplayNames)
            {
                descriptor.DisplayNames.Add(CultureInfo.GetCultureInfo(displayName.Key), displayName.Value);
            }
        }

        // Thêm Permissions
        if (input.Permissions != null)
        {
            foreach (var permission in input.Permissions)
            {
                descriptor.Permissions.Add(permission);
            }
        }

        // Thêm Redirect URIs
        if (input.RedirectUris != null)
        {
            foreach (var uri in input.RedirectUris)
            {
                descriptor.RedirectUris.Add(new Uri(uri));
            }
        }

        // Thêm Post Logout Redirect URIs
        if (input.PostLogoutRedirectUris != null)
        {
            foreach (var uri in input.PostLogoutRedirectUris)
            {
                descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
            }
        }

        // Thêm Requirements
        if (input.Requirements != null)
        {
            foreach (var requirement in input.Requirements)
            {
                descriptor.Requirements.Add(requirement);
            }
        }

        var application = await _applicationManager.CreateAsync(descriptor);
        return await MapToApplicationDtoAsync(application);
    }

    /// <summary>
    /// Cập nhật thông tin application
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Applications.Update)]
    public async Task<OpenIddictApplicationDto> UpdateAsync(string id, UpdateOpenIddictApplicationDto input)
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
            descriptor.ClientType = input.Type; // Sử dụng ClientType thay vì Type
        }

        await _applicationManager.UpdateAsync(application, descriptor);
        return await MapToApplicationDtoAsync(application);
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
            if (!scopes.Contains(scopeName))
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
            Id = await _applicationManager.GetIdAsync(application),
            ClientId = await _applicationManager.GetClientIdAsync(application),
            ClientSecret = null, // Client secret không được expose vì lý do bảo mật
            ConsentType = await _applicationManager.GetConsentTypeAsync(application),
            DisplayName = await _applicationManager.GetDisplayNameAsync(application),
            Type = await _applicationManager.GetClientTypeAsync(application) // Sử dụng GetClientTypeAsync
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

        return dto;
    }
} 