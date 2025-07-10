using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Repositories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service cho quản lý vai trò SmartBox
/// </summary>
// [Authorize(SmartBoxPermissions.Roles.Default)] // Tạm thời comment out để test
public class SmartBoxRoleAppService : SmartBoxAppService, ISmartBoxRoleAppService
{
    private readonly ISmartBoxRoleRepository _roleRepository;
    private readonly IRepository<SmartBoxRole, Guid> _roleBaseRepository;

    public SmartBoxRoleAppService(
        ISmartBoxRoleRepository roleRepository,
        IRepository<SmartBoxRole, Guid> roleBaseRepository)
    {
        _roleRepository = roleRepository;
        _roleBaseRepository = roleBaseRepository;
    }

    /// <summary>
    /// Lấy danh sách vai trò với phân trang
    /// </summary>
    public virtual async Task<PagedResultDto<SmartBoxRoleDto>> GetListAsync(GetSmartBoxRolesInput input)
    {
        var queryable = await _roleBaseRepository.GetQueryableAsync();

        // Áp dụng filter
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => 
                x.Name.Contains(input.Filter) || 
                x.DisplayName.Contains(input.Filter) ||
                (x.Description != null && x.Description.Contains(input.Filter)));
        }

        if (input.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        }

        if (input.IsSystem.HasValue)
        {
            queryable = queryable.Where(x => x.IsSystem == input.IsSystem.Value);
        }

        // Áp dụng sorting
        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            // Sử dụng dynamic sorting với validation an toàn
            var allowedSortFields = new[] { "Name", "DisplayName", "CreationTime", "DisplayOrder", "IsActive", "IsSystem" };
            var sortingParts = input.Sorting.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var validSortings = new List<string>();
            
            foreach (var part in sortingParts)
            {
                var sortField = part.Trim();
                var isDescending = sortField.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
                var fieldName = isDescending ? sortField[..^5].Trim() : sortField;
                
                if (allowedSortFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase))
                {
                    validSortings.Add(isDescending ? $"{fieldName} descending" : fieldName);
                }
            }
            
            if (validSortings.Count > 0)
            {
                queryable = queryable.OrderBy(string.Join(", ", validSortings));
            }
            else
            {
                queryable = queryable.OrderBy(x => x.DisplayOrder).ThenBy(x => x.DisplayName);
            }
        }
        else
        {
            queryable = queryable.OrderBy(x => x.DisplayOrder).ThenBy(x => x.DisplayName);
        }

        var totalCount = queryable.Count();
        var items = queryable.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<SmartBoxRoleDto>(
            totalCount,
            ObjectMapper.Map<List<SmartBoxRole>, List<SmartBoxRoleDto>>(items)
        );
    }

    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    public virtual async Task<SmartBoxRoleDto> GetAsync(Guid id)
    {
        var role = await _roleBaseRepository.GetAsync(id);
        return ObjectMapper.Map<SmartBoxRole, SmartBoxRoleDto>(role);
    }

    /// <summary>
    /// Tạo vai trò mới
    /// </summary>
    // [Authorize(SmartBoxPermissions.Roles.Create)] // Tạm thời comment out
    public virtual async Task<SmartBoxRoleDto> CreateAsync(CreateUpdateSmartBoxRoleDto input)
    {
        // Kiểm tra tên đã tồn tại chưa
        if (await _roleRepository.IsNameExistAsync(input.Name))
        {
            throw new UserFriendlyException($"Tên vai trò '{input.Name}' đã tồn tại");
        }

        if (await _roleRepository.IsDisplayNameExistAsync(input.DisplayName))
        {
            throw new UserFriendlyException($"Tên hiển thị '{input.DisplayName}' đã tồn tại");
        }

        var role = new SmartBoxRole(
            GuidGenerator.Create(),
            input.Name,
            input.DisplayName,
            input.Description,
            false, // Không phải vai trò hệ thống
            CurrentTenant.Id
        )
        {
            IsActive = input.IsActive,
            DisplayOrder = input.DisplayOrder
        };

        role = await _roleBaseRepository.InsertAsync(role, autoSave: true);
        return ObjectMapper.Map<SmartBoxRole, SmartBoxRoleDto>(role);
    }

    /// <summary>
    /// Cập nhật thông tin vai trò
    /// </summary>
    // [Authorize(SmartBoxPermissions.Roles.Edit)] // Tạm thời comment out
    public virtual async Task<SmartBoxRoleDto> UpdateAsync(Guid id, CreateUpdateSmartBoxRoleDto input)
    {
        var role = await _roleBaseRepository.GetAsync(id);

        // Không cho phép sửa vai trò hệ thống
        if (role.IsSystem)
        {
            throw new UserFriendlyException("Không thể chỉnh sửa vai trò hệ thống");
        }

        // Kiểm tra tên đã tồn tại chưa (trừ chính nó)
        if (await _roleRepository.IsNameExistAsync(input.Name, id))
        {
            throw new UserFriendlyException($"Tên vai trò '{input.Name}' đã tồn tại");
        }

        if (await _roleRepository.IsDisplayNameExistAsync(input.DisplayName, id))
        {
            throw new UserFriendlyException($"Tên hiển thị '{input.DisplayName}' đã tồn tại");
        }

        role.Name = input.Name;
        role.DisplayName = input.DisplayName;
        role.Description = input.Description;
        role.IsActive = input.IsActive;
        role.DisplayOrder = input.DisplayOrder;

        role = await _roleBaseRepository.UpdateAsync(role, autoSave: true);
        return ObjectMapper.Map<SmartBoxRole, SmartBoxRoleDto>(role);
    }

    /// <summary>
    /// Xóa vai trò (chỉ có thể xóa role không phải hệ thống)
    /// </summary>
    // [Authorize(SmartBoxPermissions.Roles.Delete)] // Tạm thời comment out
    public virtual async Task DeleteAsync(Guid id)
    {
        var role = await _roleBaseRepository.GetAsync(id);

        if (role.IsSystem)
        {
            throw new UserFriendlyException("Không thể xóa vai trò hệ thống");
        }

        // TODO: Kiểm tra xem có user nào đang sử dụng role này không
        
        await _roleBaseRepository.DeleteAsync(id, autoSave: true);
    }

    /// <summary>
    /// Lấy danh sách vai trò hoạt động cho dropdown
    /// </summary>
    public virtual async Task<ListResultDto<SmartBoxRoleDto>> GetActiveRolesAsync()
    {
        var roles = await _roleRepository.GetActiveRolesAsync();
        return new ListResultDto<SmartBoxRoleDto>(
            ObjectMapper.Map<List<SmartBoxRole>, List<SmartBoxRoleDto>>(roles)
        );
    }

    /// <summary>
    /// Lấy danh sách vai trò của user
    /// </summary>
    public virtual async Task<ListResultDto<SmartBoxRoleDto>> GetRolesByUserIdAsync(Guid userId)
    {
        var roles = await _roleRepository.GetRolesByUserIdAsync(userId);
        return new ListResultDto<SmartBoxRoleDto>(
            ObjectMapper.Map<List<SmartBoxRole>, List<SmartBoxRoleDto>>(roles)
        );
    }

    /// <summary>
    /// Kiểm tra tên vai trò đã tồn tại chưa
    /// </summary>
    public virtual async Task<bool> IsNameExistAsync(string name, Guid? excludeId = null)
    {
        return await _roleRepository.IsNameExistAsync(name, excludeId);
    }

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    public virtual async Task<bool> IsDisplayNameExistAsync(string displayName, Guid? excludeId = null)
    {
        return await _roleRepository.IsDisplayNameExistAsync(displayName, excludeId);
    }
} 