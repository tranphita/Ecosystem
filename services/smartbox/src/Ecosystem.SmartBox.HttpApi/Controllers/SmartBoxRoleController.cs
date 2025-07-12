using System;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.SmartBox.Controllers;

/// <summary>
/// API Controller cho quản lý vai trò SmartBox
/// </summary>
[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/smartbox/roles")]
[Authorize]
public class SmartBoxRoleController(ISmartBoxRoleAppService roleAppService) : SmartBoxController, ISmartBoxRoleAppService
{
    /// <summary>
    /// Lấy danh sách vai trò với phân trang
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<SmartBoxRoleDto>> GetListAsync(GetSmartBoxRolesInput input)
    {
        return roleAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual Task<SmartBoxRoleDto> GetAsync(Guid id)
    {
        return roleAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo vai trò mới
    /// </summary>
    [HttpPost]
    public virtual Task<SmartBoxRoleDto> CreateAsync(CreateUpdateSmartBoxRoleDto input)
    {
        return roleAppService.CreateAsync(input);
    }

    /// <summary>
    /// Cập nhật thông tin vai trò
    /// </summary>
    [HttpPut("{id}")]
    public virtual Task<SmartBoxRoleDto> UpdateAsync(Guid id, CreateUpdateSmartBoxRoleDto input)
    {
        return roleAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Xóa vai trò
    /// </summary>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return roleAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Lấy danh sách vai trò hoạt động cho dropdown
    /// </summary>
    [HttpGet("active")]
    public virtual Task<ListResultDto<SmartBoxRoleDto>> GetActiveRolesAsync()
    {
        return roleAppService.GetActiveRolesAsync();
    }

    /// <summary>
    /// Lấy danh sách vai trò của user
    /// </summary>
    [HttpGet("by-user/{userId}")]
    public virtual Task<ListResultDto<SmartBoxRoleDto>> GetRolesByUserIdAsync(Guid userId)
    {
        return roleAppService.GetRolesByUserIdAsync(userId);
    }

    /// <summary>
    /// Kiểm tra tên vai trò đã tồn tại chưa
    /// </summary>
    [HttpGet("check-name")]
    public virtual Task<bool> IsNameExistAsync(string name, Guid? excludeId = null)
    {
        return roleAppService.IsNameExistAsync(name, excludeId);
    }

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    [HttpGet("check-display-name")]
    public virtual Task<bool> IsDisplayNameExistAsync(string displayName, Guid? excludeId = null)
    {
        return roleAppService.IsDisplayNameExistAsync(displayName, excludeId);
    }
}