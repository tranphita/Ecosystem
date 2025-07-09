using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.SmartBox.Controllers;

/// <summary>
/// API Controller cho quản lý người dùng SmartBox
/// </summary>
[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/smartbox/users")]
public class SmartBoxUserController : SmartBoxController, ISmartBoxUserAppService
{
    private readonly ISmartBoxUserAppService _userAppService;

    public SmartBoxUserController(ISmartBoxUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    /// <summary>
    /// Lấy danh sách người dùng với phân trang
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetListAsync(GetSmartBoxUsersInput input)
    {
        return _userAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual Task<SmartBoxUserDto> GetAsync(Guid id)
    {
        return _userAppService.GetAsync(id);
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại
    /// </summary>
    [HttpGet("current")]
    public virtual Task<SmartBoxUserDto> GetCurrentUserAsync()
    {
        return _userAppService.GetCurrentUserAsync();
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    [HttpPut("{id}")]
    public virtual Task<SmartBoxUserDto> UpdateAsync(Guid id, CreateUpdateSmartBoxUserDto input)
    {
        return _userAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân của user hiện tại
    /// </summary>
    [HttpPut("current")]
    public virtual Task<SmartBoxUserDto> UpdateCurrentUserAsync(CreateUpdateSmartBoxUserDto input)
    {
        return _userAppService.UpdateCurrentUserAsync(input);
    }

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _userAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Kích hoạt/Vô hiệu hóa người dùng
    /// </summary>
    [HttpPut("{id}/set-active")]
    public virtual Task<SmartBoxUserDto> SetActiveAsync(Guid id, bool isActive)
    {
        return _userAppService.SetActiveAsync(id, isActive);
    }

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    [HttpPost("{userId}/assign-roles")]
    public virtual Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        return _userAppService.AssignRolesToUserAsync(userId, roleIds);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo công ty
    /// </summary>
    [HttpGet("by-company/{companyId}")]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetUsersByCompanyAsync(Guid companyId, GetSmartBoxUsersInput input)
    {
        return _userAppService.GetUsersByCompanyAsync(companyId, input);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò
    /// </summary>
    [HttpGet("by-role/{roleId}")]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetUsersByRoleAsync(Guid roleId, GetSmartBoxUsersInput input)
    {
        return _userAppService.GetUsersByRoleAsync(roleId, input);
    }

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    [HttpGet("check-username")]
    public virtual Task<bool> IsUserNameExistAsync(string userName, Guid? excludeId = null)
    {
        return _userAppService.IsUserNameExistAsync(userName, excludeId);
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    [HttpGet("check-email")]
    public virtual Task<bool> IsEmailExistAsync(string email, Guid? excludeId = null)
    {
        return _userAppService.IsEmailExistAsync(email, excludeId);
    }

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    [HttpGet("check-employee-code")]
    public virtual Task<bool> IsEmployeeCodeExistAsync(string employeeCode, Guid? excludeId = null)
    {
        return _userAppService.IsEmployeeCodeExistAsync(employeeCode, excludeId);
    }

    /// <summary>
    /// Đồng bộ thông tin user từ AuthServer
    /// </summary>
    [HttpPost("sync-current")]
    public virtual Task<SmartBoxUserDto> SyncCurrentUserAsync()
    {
        return _userAppService.SyncCurrentUserAsync();
    }

    /// <summary>
    /// Cập nhật avatar cho user hiện tại
    /// </summary>
    [HttpPut("current/avatar")]
    public virtual Task<SmartBoxUserDto> UpdateAvatarAsync([FromBody] string avatar)
    {
        return _userAppService.UpdateAvatarAsync(avatar);
    }
} 