using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Repositories;
using Ecosystem.SmartBox.Services;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace Ecosystem.SmartBox.Users;

[Authorize(SmartBoxPermissions.Users.Default)]
public class SmartBoxUserAppService(
    ISmartBoxUserRepository userRepository,
    IRepository<SmartBoxUser, Guid> userBaseRepository,
    IRepository<SmartBoxUserRole> userRoleRepository,
    IUserBidirectionalSyncService bidirectionalSyncService,
    IDistributedEventBus distributedEventBus) : SmartBoxAppService, ISmartBoxUserAppService
{
    public virtual async Task<PagedResultDto<SmartBoxUserDto>> GetListAsync(GetSmartBoxUsersInput input)
    {
        var queryable = await userBaseRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x =>
                x.FullName != null && x.FullName.Contains(input.Filter) ||
                x.UserName.Contains(input.Filter) ||
                x.Email.Contains(input.Filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(input.Filter)));
        }

        if (input.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        }

        if (input.CompanyId.HasValue)
        {
            queryable = queryable.Where(x => x.CompanyId == input.CompanyId.Value);
        }

        var totalCount = queryable.Count();
        var items = queryable.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<SmartBoxUserDto>(
            totalCount,
            ObjectMapper.Map<List<SmartBoxUser>, List<SmartBoxUserDto>>(items)
        );
    }

    public virtual async Task<SmartBoxUserDto> GetAsync(Guid id)
    {
        var user = await userRepository.GetWithDetailsAsync(id);
        return user == null
            ? throw new EntityNotFoundException(typeof(SmartBoxUser), id)
            : ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    public virtual async Task<SmartBoxUserDto> GetCurrentUserAsync()
    {
        var currentUserId = CurrentUser.GetId();
        var user = await userRepository.FindByAuthUserIdAsync(currentUserId);

        return user == null
            ? throw new BusinessException("USER_NOT_FOUND", "Không tìm th?y thông tin ng??i dùng hi?n t?i")
            : ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    [Authorize(SmartBoxPermissions.Users.Create)]
    public virtual async Task<SmartBoxUserDto> CreateAsync(CreateSmartBoxUserDto input)
    {
        if (await userRepository.IsUserNameExistAsync(input.UserName))
        {
            throw new BusinessException("USERNAME_ALREADY_EXISTS", $"Tên ??ng nh?p '{input.UserName}' ?ã t?n t?i");
        }

        if (await userRepository.IsEmailExistAsync(input.Email))
        {
            throw new BusinessException("EMAIL_ALREADY_EXISTS", $"Email '{input.Email}' ?ã t?n t?i");
        }

        var (authUserId, smartBoxUser) = await bidirectionalSyncService.CreateUserInIdentityAsync(
            input.UserName,
            input.Email,
            input.Password,
            input.FullName,
            input.PhoneNumber,
            input.IsActive,
            input.RequirePasswordChange
        );

        smartBoxUser.DateOfBirth = input.DateOfBirth;
        smartBoxUser.Gender = input.Gender;
        smartBoxUser.Avatar = input.Avatar;
        smartBoxUser.CompanyId = input.CompanyId;
        smartBoxUser.Position = input.Position;
        smartBoxUser.Department = input.Department;
        smartBoxUser.EmployeeCode = input.EmployeeCode;
        smartBoxUser.StartDate = input.StartDate;
        smartBoxUser.Salary = input.Salary;
        smartBoxUser.Address = input.Address;
        smartBoxUser.Notes = input.Notes;

        if (input.RoleIds.Count != 0)
        {
            await UpdateUserRolesAsync(smartBoxUser.Id, input.RoleIds);
        }

        smartBoxUser = await userBaseRepository.UpdateAsync(smartBoxUser, autoSave: true);

        await distributedEventBus.PublishAsync(new UserCreatedIntegrationEvent
        {
            SmartBoxUserId = smartBoxUser.Id,
            AuthUserId = authUserId,
            UserName = input.UserName,
            Email = input.Email,
            FullName = input.FullName,
            PhoneNumber = input.PhoneNumber,
            TenantId = CurrentTenant.Id
        });

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(smartBoxUser);
    }

    [Authorize(SmartBoxPermissions.Users.Edit)]
    public virtual async Task<SmartBoxUserDto> UpdateAsync(Guid id, CreateUpdateSmartBoxUserDto input)
    {
        var user = await userBaseRepository.GetAsync(id);

        user.FullName = input.FullName;
        user.PhoneNumber = input.PhoneNumber;
        user.DateOfBirth = input.DateOfBirth;
        user.Gender = input.Gender;
        user.Avatar = input.Avatar;
        user.CompanyId = input.CompanyId;
        user.Position = input.Position;
        user.Department = input.Department;
        user.EmployeeCode = input.EmployeeCode;
        user.StartDate = input.StartDate;
        user.Salary = input.Salary;
        user.Address = input.Address;
        user.Notes = input.Notes;
        user.IsActive = input.IsActive;

        if (input.RoleIds.Count != 0)
        {
            await UpdateUserRolesAsync(user.Id, input.RoleIds);
        }

        user = await userBaseRepository.UpdateAsync(user, autoSave: true);

        try
        {
            await bidirectionalSyncService.SyncSmartBoxUserToIdentityAsync(user, true, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "L?i khi ??ng b? user {AuthUserId} sang Identity Service", user.AuthUserId);
        }

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    public virtual async Task<SmartBoxUserDto> UpdateCurrentUserAsync(CreateUpdateSmartBoxUserDto input)
    {
        var currentUserId = CurrentUser.GetId();
        var user = await userRepository.FindByAuthUserIdAsync(currentUserId) ?? 
            throw new BusinessException("USER_NOT_FOUND", "Không tìm th?y thông tin ng??i dùng hi?n t?i");

        user.FullName = input.FullName;
        user.PhoneNumber = input.PhoneNumber;
        user.DateOfBirth = input.DateOfBirth;
        user.Gender = input.Gender;
        user.Avatar = input.Avatar;
        user.Address = input.Address;

        user = await userBaseRepository.UpdateAsync(user, autoSave: true);
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    [Authorize(SmartBoxPermissions.Users.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var user = await userBaseRepository.GetAsync(id);
        await userBaseRepository.DeleteAsync(id, autoSave: true);

        try
        {
            await bidirectionalSyncService.SetUserActiveInIdentityAsync(user.AuthUserId, false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "L?i khi vô hi?u hóa user {AuthUserId} trong Identity Service", user.AuthUserId);
        }
    }

    [Authorize(SmartBoxPermissions.Users.Edit)]
    public virtual async Task<SmartBoxUserDto> SetActiveAsync(Guid id, bool isActive)
    {
        var user = await userBaseRepository.GetAsync(id);
        user.IsActive = isActive;
        user = await userBaseRepository.UpdateAsync(user, autoSave: true);

        try
        {
            await bidirectionalSyncService.SetUserActiveInIdentityAsync(user.AuthUserId, isActive);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "L?i khi c?p nh?t tr?ng thái user {AuthUserId} trong Identity Service", user.AuthUserId);
        }

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    [Authorize(SmartBoxPermissions.Users.ManageRoles)]
    public virtual Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        return UpdateUserRolesAsync(userId, roleIds);
    }

    public virtual Task<PagedResultDto<SmartBoxUserDto>> GetUsersByCompanyAsync(Guid companyId, GetSmartBoxUsersInput input)
    {
        input.CompanyId = companyId;
        return GetListAsync(input);
    }

    public virtual async Task<PagedResultDto<SmartBoxUserDto>> GetUsersByRoleAsync(Guid roleId, GetSmartBoxUsersInput input)
    {
        var users = await userRepository.GetUsersByRoleAsync(
            roleId,
            input.Filter,
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "FullName"
        );

        var totalUsers = await userRepository.GetUsersByRoleAsync(roleId, input.Filter);

        return new PagedResultDto<SmartBoxUserDto>(
            totalUsers.Count,
            ObjectMapper.Map<List<SmartBoxUser>, List<SmartBoxUserDto>>(users)
        );
    }

    public virtual Task<bool> IsUserNameExistAsync(string userName, Guid? excludeId = null)
    {
        return userRepository.IsUserNameExistAsync(userName, excludeId);
    }

    public virtual Task<bool> IsEmailExistAsync(string email, Guid? excludeId = null)
    {
        return userRepository.IsEmailExistAsync(email, excludeId);
    }

    public virtual Task<bool> IsEmployeeCodeExistAsync(string employeeCode, Guid? excludeId = null)
    {
        return userRepository.IsEmployeeCodeExistAsync(employeeCode, excludeId);
    }

    public virtual async Task<SmartBoxUserDto> SyncCurrentUserAsync()
    {
        var currentUserId = CurrentUser.GetId();
        var currentUserName = CurrentUser.UserName;
        var currentEmail = CurrentUser.Email;

        if (string.IsNullOrEmpty(currentUserName) || string.IsNullOrEmpty(currentEmail))
        {
            throw new BusinessException("INVALID_USER_INFO", "Thông tin ng??i dùng không h?p l?");
        }

        var user = await bidirectionalSyncService.SyncIdentityUserToSmartBoxAsync(
            currentUserId,
            currentUserName,
            currentEmail,
            CurrentUser.Name,
            CurrentUser.PhoneNumber,
            true
        );

        user.UpdateLastLoginTime();
        await userBaseRepository.UpdateAsync(user, autoSave: true);

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    public virtual async Task<SmartBoxUserDto> UpdateAvatarAsync(string avatar)
    {
        var currentUserId = CurrentUser.GetId();
        var user = await userRepository.FindByAuthUserIdAsync(currentUserId) ?? 
            throw new BusinessException("USER_NOT_FOUND", "Không tìm th?y thông tin ng??i dùng hi?n t?i");
        
        user.Avatar = avatar;
        user = await userBaseRepository.UpdateAsync(user, autoSave: true);

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    private async Task UpdateUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        var currentUserRoles = await userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        await userRoleRepository.DeleteManyAsync(currentUserRoles);

        if (roleIds.Count != 0)
        {
            var newUserRoles = roleIds.Select(roleId => new SmartBoxUserRole(
                userId,
                roleId,
                CurrentUser.Id,
                CurrentTenant.Id
            )).ToList();

            await userRoleRepository.InsertManyAsync(newUserRoles, autoSave: true);
        }
    }
}