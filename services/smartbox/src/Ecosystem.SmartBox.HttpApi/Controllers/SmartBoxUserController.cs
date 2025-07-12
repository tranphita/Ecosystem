using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Ecosystem.SmartBox.Controllers;

/// <summary>
/// API Controller cho quản lý người dùng SmartBox
/// </summary>
[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/smartbox/users")]
[Authorize]
public class SmartBoxUserController(ISmartBoxUserAppService userAppService) : SmartBoxController, ISmartBoxUserAppService
{
    /// <summary>
    /// Lấy danh sách người dùng với phân trang
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetListAsync(GetSmartBoxUsersInput input)
    {
        return userAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual Task<SmartBoxUserDto> GetAsync(Guid id)
    {
        return userAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo người dùng mới (bao gồm tạo account trong Identity Service)
    /// </summary>
    [HttpPost]
    public virtual Task<SmartBoxUserDto> CreateAsync(CreateSmartBoxUserDto input)
    {
        return userAppService.CreateAsync(input);
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại
    /// </summary>
    [HttpGet("current")]
    public virtual Task<SmartBoxUserDto> GetCurrentUserAsync()
    {
        return userAppService.GetCurrentUserAsync();
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    [HttpPut("{id}")]
    public virtual Task<SmartBoxUserDto> UpdateAsync(Guid id, CreateUpdateSmartBoxUserDto input)
    {
        return userAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân của user hiện tại
    /// </summary>
    [HttpPut("current")]
    public virtual Task<SmartBoxUserDto> UpdateCurrentUserAsync(CreateUpdateSmartBoxUserDto input)
    {
        return userAppService.UpdateCurrentUserAsync(input);
    }

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return userAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Kích hoạt/Vô hiệu hóa người dùng
    /// </summary>
    [HttpPut("{id}/set-active")]
    public virtual Task<SmartBoxUserDto> SetActiveAsync(Guid id, bool isActive)
    {
        return userAppService.SetActiveAsync(id, isActive);
    }

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    [HttpPost("{userId}/assign-roles")]
    public virtual Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        return userAppService.AssignRolesToUserAsync(userId, roleIds);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo công ty
    /// </summary>
    [HttpGet("by-company/{companyId}")]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetUsersByCompanyAsync(Guid companyId, GetSmartBoxUsersInput input)
    {
        return userAppService.GetUsersByCompanyAsync(companyId, input);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò
    /// </summary>
    [HttpGet("by-role/{roleId}")]
    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetUsersByRoleAsync(Guid roleId, GetSmartBoxUsersInput input)
    {
        return userAppService.GetUsersByRoleAsync(roleId, input);
    }

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    [HttpGet("check-username")]
    public virtual Task<bool> IsUserNameExistAsync(string userName, Guid? excludeId = null)
    {
        return userAppService.IsUserNameExistAsync(userName, excludeId);
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    [HttpGet("check-email")]
    public virtual Task<bool> IsEmailExistAsync(string email, Guid? excludeId = null)
    {
        return userAppService.IsEmailExistAsync(email, excludeId);
    }

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    [HttpGet("check-employee-code")]
    public virtual Task<bool> IsEmployeeCodeExistAsync(string employeeCode, Guid? excludeId = null)
    {
        return userAppService.IsEmployeeCodeExistAsync(employeeCode, excludeId);
    }

    /// <summary>
    /// Đồng bộ thông tin user từ AuthServer
    /// </summary>
    [HttpPost("sync-current")]
    public virtual Task<SmartBoxUserDto> SyncCurrentUserAsync()
    {
        return userAppService.SyncCurrentUserAsync();
    }

    /// <summary>
    /// Cập nhật avatar cho user hiện tại
    /// </summary>
    [HttpPut("current/avatar")]
    public virtual Task<SmartBoxUserDto> UpdateAvatarAsync([FromBody] string avatar)
    {
        return userAppService.UpdateAvatarAsync(avatar);
    }
}