using System;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.SmartBox.Controllers;

/// <summary>
/// API Controller cho quản lý vai trò SmartBox
/// </summary>
[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/smartbox/roles")]
public class SmartBoxRoleController : SmartBoxController, ISmartBoxRoleAppService
{
    private readonly ISmartBoxRoleAppService _roleAppService;

    public SmartBoxRoleController(ISmartBoxRoleAppService roleAppService)
    {
        _roleAppService = roleAppService;
    }

    /// <summary>
    /// Lấy danh sách vai trò với phân trang
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<SmartBoxRoleDto>> GetListAsync(GetSmartBoxRolesInput input)
    {
        return _roleAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual Task<SmartBoxRoleDto> GetAsync(Guid id)
    {
        return _roleAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo vai trò mới
    /// </summary>
    [HttpPost]
    public virtual Task<SmartBoxRoleDto> CreateAsync(CreateUpdateSmartBoxRoleDto input)
    {
        return _roleAppService.CreateAsync(input);
    }

    /// <summary>
    /// Cập nhật thông tin vai trò
    /// </summary>
    [HttpPut("{id}")]
    public virtual Task<SmartBoxRoleDto> UpdateAsync(Guid id, CreateUpdateSmartBoxRoleDto input)
    {
        return _roleAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Xóa vai trò
    /// </summary>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _roleAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Lấy danh sách vai trò hoạt động cho dropdown
    /// </summary>
    [HttpGet("active")]
    public virtual Task<ListResultDto<SmartBoxRoleDto>> GetActiveRolesAsync()
    {
        return _roleAppService.GetActiveRolesAsync();
    }

    /// <summary>
    /// Lấy danh sách vai trò của user
    /// </summary>
    [HttpGet("by-user/{userId}")]
    public virtual Task<ListResultDto<SmartBoxRoleDto>> GetRolesByUserIdAsync(Guid userId)
    {
        return _roleAppService.GetRolesByUserIdAsync(userId);
    }

    /// <summary>
    /// Kiểm tra tên vai trò đã tồn tại chưa
    /// </summary>
    [HttpGet("check-name")]
    public virtual Task<bool> IsNameExistAsync(string name, Guid? excludeId = null)
    {
        return _roleAppService.IsNameExistAsync(name, excludeId);
    }

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    [HttpGet("check-display-name")]
    public virtual Task<bool> IsDisplayNameExistAsync(string displayName, Guid? excludeId = null)
    {
        return _roleAppService.IsDisplayNameExistAsync(displayName, excludeId);
    }
} 