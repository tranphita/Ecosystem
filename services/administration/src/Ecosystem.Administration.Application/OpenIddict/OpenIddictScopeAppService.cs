using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ecosystem.Administration.OpenIddict;
using Ecosystem.Administration.Permissions;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Application Service cho quản lý OpenIddict Scopes
/// </summary>
[Authorize(AdministrationPermissions.OpenIddict.Scopes.Default)]
public class OpenIddictScopeAppService : AdministrationAppService, IOpenIddictScopeAppService
{
    private readonly IOpenIddictScopeManager _scopeManager;

    public OpenIddictScopeAppService(IOpenIddictScopeManager scopeManager)
    {
        _scopeManager = scopeManager;
    }

    /// <summary>
    /// Lấy danh sách scopes với phân trang và tìm kiếm
    /// </summary>
    public async Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopeListDto input)
    {
        var scopes = new List<object>();
        await foreach (var scope in _scopeManager.ListAsync())
        {
            scopes.Add(scope);
        }

        var items = new List<OpenIddictScopeDto>();
        foreach (var scope in scopes.Skip(input.SkipCount).Take(input.MaxResultCount))
        {
            var dto = await MapToScopeDtoAsync(scope);
            
            // Áp dụng bộ lọc tìm kiếm
            if (!string.IsNullOrEmpty(input.Filter))
            {
                if (!dto.Name.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) &&
                    !(dto.DisplayName?.Contains(input.Filter, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    continue;
                }
            }

            items.Add(dto);
        }

        return new PagedResultDto<OpenIddictScopeDto>(
            scopes.Count,
            items
        );
    }

    /// <summary>
    /// Lấy thông tin chi tiết một scope theo ID
    /// </summary>
    public async Task<OpenIddictScopeDto> GetAsync(string id)
    {
        var scope = await _scopeManager.FindByIdAsync(id);
        if (scope == null)
        {
            throw new Volo.Abp.UserFriendlyException("Scope not found");
        }

        return await MapToScopeDtoAsync(scope);
    }

    /// <summary>
    /// Tạo mới một scope
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Scopes.Create)]
    public async Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input)
    {
        // Kiểm tra tên scope đã tồn tại
        if (await IsScopeNameExistsAsync(input.Name))
        {
            throw new Volo.Abp.UserFriendlyException("Scope name already exists");
        }

        var descriptor = new OpenIddictScopeDescriptor
        {
            Name = input.Name,
            DisplayName = input.DisplayName,
            Description = input.Description
        };

        // Thêm DisplayNames (đa ngôn ngữ)
        if (input.DisplayNames != null)
        {
            foreach (var displayName in input.DisplayNames)
            {
                descriptor.DisplayNames.Add(CultureInfo.GetCultureInfo(displayName.Key), displayName.Value);
            }
        }

        // Thêm Descriptions (đa ngôn ngữ)
        if (input.Descriptions != null)
        {
            foreach (var description in input.Descriptions)
            {
                descriptor.Descriptions.Add(CultureInfo.GetCultureInfo(description.Key), description.Value);
            }
        }

        // Thêm Resources
        if (input.Resources != null)
        {
            foreach (var resource in input.Resources)
            {
                descriptor.Resources.Add(resource);
            }
        }

        var scope = await _scopeManager.CreateAsync(descriptor);
        return await MapToScopeDtoAsync(scope);
    }

    /// <summary>
    /// Cập nhật thông tin scope
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Scopes.Update)]
    public async Task<OpenIddictScopeDto> UpdateAsync(string id, UpdateOpenIddictScopeDto input)
    {
        var scope = await _scopeManager.FindByIdAsync(id);
        if (scope == null)
        {
            throw new Volo.Abp.UserFriendlyException("Scope not found");
        }

        var descriptor = new OpenIddictScopeDescriptor();
        await _scopeManager.PopulateAsync(descriptor, scope);

        // Cập nhật các thuộc tính
        if (!string.IsNullOrEmpty(input.DisplayName))
        {
            descriptor.DisplayName = input.DisplayName;
        }

        if (!string.IsNullOrEmpty(input.Description))
        {
            descriptor.Description = input.Description;
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

        // Cập nhật Descriptions
        if (input.Descriptions != null)
        {
            descriptor.Descriptions.Clear();
            foreach (var description in input.Descriptions)
            {
                descriptor.Descriptions.Add(CultureInfo.GetCultureInfo(description.Key), description.Value);
            }
        }

        // Cập nhật Resources
        if (input.Resources != null)
        {
            descriptor.Resources.Clear();
            foreach (var resource in input.Resources)
            {
                descriptor.Resources.Add(resource);
            }
        }

        await _scopeManager.UpdateAsync(scope, descriptor);
        return await MapToScopeDtoAsync(scope);
    }

    /// <summary>
    /// Xóa scope theo ID
    /// </summary>
    [Authorize(AdministrationPermissions.OpenIddict.Scopes.Delete)]
    public async Task DeleteAsync(string id)
    {
        var scope = await _scopeManager.FindByIdAsync(id);
        if (scope == null)
        {
            throw new Volo.Abp.UserFriendlyException("Scope not found");
        }

        await _scopeManager.DeleteAsync(scope);
    }

    /// <summary>
    /// Kiểm tra tên scope đã tồn tại hay chưa
    /// </summary>
    public async Task<bool> IsScopeNameExistsAsync(string name)
    {
        var scope = await _scopeManager.FindByNameAsync(name);
        return scope != null;
    }

    /// <summary>
    /// Lấy danh sách resources có sẵn cho scopes
    /// </summary>
    public Task<string[]> GetAvailableResourcesAsync()
    {
        // Trả về danh sách resources chuẩn
        return Task.FromResult(new[]
        {
            "administration_api",
            "identity_api",
            "saas_api",
            "smartbox_api"
        });
    }

    /// <summary>
    /// Ánh xạ từ OpenIddict Scope entity sang DTO
    /// </summary>
    private async Task<OpenIddictScopeDto> MapToScopeDtoAsync(object scope)
    {
        var dto = new OpenIddictScopeDto
        {
            Id = await _scopeManager.GetIdAsync(scope),
            Name = await _scopeManager.GetNameAsync(scope),
            DisplayName = await _scopeManager.GetDisplayNameAsync(scope),
            Description = await _scopeManager.GetDescriptionAsync(scope)
        };

        // Ánh xạ DisplayNames
        var displayNames = await _scopeManager.GetDisplayNamesAsync(scope);
        if (displayNames.Any())
        {
            dto.DisplayNames = displayNames.ToDictionary(x => x.Key.Name, x => x.Value);
        }

        // Ánh xạ Descriptions
        var descriptions = await _scopeManager.GetDescriptionsAsync(scope);
        if (descriptions.Any())
        {
            dto.Descriptions = descriptions.ToDictionary(x => x.Key.Name, x => x.Value);
        }

        // Ánh xạ Resources
        var resources = await _scopeManager.GetResourcesAsync(scope);
        if (resources.Any())
        {
            dto.Resources = resources.ToList();
        }

        return dto;
    }
} 