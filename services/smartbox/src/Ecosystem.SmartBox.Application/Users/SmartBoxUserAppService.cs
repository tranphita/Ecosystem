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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service cho quản lý người dùng SmartBox
/// </summary>
// [Authorize(SmartBoxPermissions.Users.Default)] // Tạm comment out để test
public class SmartBoxUserAppService : SmartBoxAppService, ISmartBoxUserAppService
{
    private readonly ISmartBoxUserRepository _userRepository;
    private readonly ISmartBoxRoleRepository _roleRepository;
    private readonly IRepository<SmartBoxUser, Guid> _userBaseRepository;
    private readonly IRepository<SmartBoxUserRole> _userRoleRepository;

    public SmartBoxUserAppService(
        ISmartBoxUserRepository userRepository,
        ISmartBoxRoleRepository roleRepository,
        IRepository<SmartBoxUser, Guid> userBaseRepository,
        IRepository<SmartBoxUserRole> userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userBaseRepository = userBaseRepository;
        _userRoleRepository = userRoleRepository;
    }

    /// <summary>
    /// Lấy danh sách người dùng với phân trang
    /// </summary>
    public virtual async Task<PagedResultDto<SmartBoxUserDto>> GetListAsync(GetSmartBoxUsersInput input)
    {
        var queryable = await _userBaseRepository.GetQueryableAsync();

        // Áp dụng filter
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

        if (!string.IsNullOrWhiteSpace(input.Department))
        {
            queryable = queryable.Where(x => x.Department != null && x.Department.Contains(input.Department));
        }

        if (!string.IsNullOrWhiteSpace(input.Position))
        {
            queryable = queryable.Where(x => x.Position != null && x.Position.Contains(input.Position));
        }

        // Áp dụng sorting
        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            var allowedSortFields = new[] { "FullName", "UserName", "Email", "CreationTime", "LastLoginTime", "IsActive" };
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
                queryable = queryable.OrderBy(x => x.FullName);
            }
        }
        else
        {
            queryable = queryable.OrderBy(x => x.FullName);
        }

        var totalCount = queryable.Count();
        var items = queryable.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<SmartBoxUserDto>(
            totalCount,
            ObjectMapper.Map<List<SmartBoxUser>, List<SmartBoxUserDto>>(items)
        );
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    public virtual async Task<SmartBoxUserDto> GetAsync(Guid id)
    {
        var user = await _userRepository.GetWithDetailsAsync(id);
        if (user == null)
        {
            throw new EntityNotFoundException(typeof(SmartBoxUser), id);
        }
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại
    /// </summary>
    public virtual async Task<SmartBoxUserDto> GetCurrentUserAsync()
    {
        var currentUserId = CurrentUser.GetId();
        var user = await _userRepository.FindByAuthUserIdAsync(currentUserId);
        
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "Không tìm thấy thông tin người dùng hiện tại");
        }

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    // [Authorize(SmartBoxPermissions.Users.Edit)] // Tạm comment out
    public virtual async Task<SmartBoxUserDto> UpdateAsync(Guid id, CreateUpdateSmartBoxUserDto input)
    {
        var user = await _userBaseRepository.GetAsync(id);

        // Cập nhật thông tin cơ bản
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

        // Cập nhật roles nếu có
        if (input.RoleIds.Any())
        {
            await UpdateUserRolesAsync(user.Id, input.RoleIds);
        }

        user = await _userBaseRepository.UpdateAsync(user, autoSave: true);
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân của user hiện tại
    /// </summary>
    public virtual async Task<SmartBoxUserDto> UpdateCurrentUserAsync(CreateUpdateSmartBoxUserDto input)
    {
        var currentUserId = CurrentUser.GetId();
        var user = await _userRepository.FindByAuthUserIdAsync(currentUserId);
        
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "Không tìm thấy thông tin người dùng hiện tại");
        }

        // Chỉ cho phép cập nhật một số thông tin cá nhân
        user.FullName = input.FullName;
        user.PhoneNumber = input.PhoneNumber;
        user.DateOfBirth = input.DateOfBirth;
        user.Gender = input.Gender;
        user.Avatar = input.Avatar;
        user.Address = input.Address;

        user = await _userBaseRepository.UpdateAsync(user, autoSave: true);
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    // [Authorize(SmartBoxPermissions.Users.Delete)] // Tạm comment out
    public virtual async Task DeleteAsync(Guid id)
    {
        await _userBaseRepository.DeleteAsync(id, autoSave: true);
    }

    /// <summary>
    /// Kích hoạt/Vô hiệu hóa người dùng
    /// </summary>
    // [Authorize(SmartBoxPermissions.Users.Edit)] // Tạm comment out
    public virtual async Task<SmartBoxUserDto> SetActiveAsync(Guid id, bool isActive)
    {
        var user = await _userBaseRepository.GetAsync(id);
        user.IsActive = isActive;
        
        user = await _userBaseRepository.UpdateAsync(user, autoSave: true);
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    // [Authorize(SmartBoxPermissions.Users.ManageRoles)] // Tạm comment out
    public virtual async Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        await UpdateUserRolesAsync(userId, roleIds);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo công ty
    /// </summary>
    public virtual async Task<PagedResultDto<SmartBoxUserDto>> GetUsersByCompanyAsync(Guid companyId, GetSmartBoxUsersInput input)
    {
        input.CompanyId = companyId;
        return await GetListAsync(input);
    }

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò
    /// </summary>
    public virtual async Task<PagedResultDto<SmartBoxUserDto>> GetUsersByRoleAsync(Guid roleId, GetSmartBoxUsersInput input)
    {
        input.RoleId = roleId;
        
        var users = await _userRepository.GetUsersByRoleAsync(
            roleId,
            input.Filter,
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "FullName"
        );

        // Đếm tổng số
        var totalUsers = await _userRepository.GetUsersByRoleAsync(roleId, input.Filter);
        
        return new PagedResultDto<SmartBoxUserDto>(
            totalUsers.Count,
            ObjectMapper.Map<List<SmartBoxUser>, List<SmartBoxUserDto>>(users)
        );
    }

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    public virtual async Task<bool> IsUserNameExistAsync(string userName, Guid? excludeId = null)
    {
        return await _userRepository.IsUserNameExistAsync(userName, excludeId);
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    public virtual async Task<bool> IsEmailExistAsync(string email, Guid? excludeId = null)
    {
        return await _userRepository.IsEmailExistAsync(email, excludeId);
    }

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    public virtual async Task<bool> IsEmployeeCodeExistAsync(string employeeCode, Guid? excludeId = null)
    {
        return await _userRepository.IsEmployeeCodeExistAsync(employeeCode, excludeId);
    }

    /// <summary>
    /// Đồng bộ thông tin user từ AuthServer
    /// </summary>
    public virtual async Task<SmartBoxUserDto> SyncCurrentUserAsync()
    {
        var currentUserId = CurrentUser.GetId();
        var currentUserName = CurrentUser.UserName;
        var currentEmail = CurrentUser.Email;

        if (string.IsNullOrEmpty(currentUserName) || string.IsNullOrEmpty(currentEmail))
        {
            throw new BusinessException("INVALID_USER_INFO", "Thông tin người dùng không hợp lệ");
        }

        var user = await _userRepository.FindByAuthUserIdAsync(currentUserId);
        
        if (user == null)
        {
            // Tạo user mới nếu chưa tồn tại
            user = new SmartBoxUser(
                GuidGenerator.Create(),
                currentUserId,
                currentUserName,
                currentEmail,
                null, // fullName
                null, // companyId
                CurrentTenant.Id
            );
            
            user = await _userBaseRepository.InsertAsync(user, autoSave: true);
        }
        else
        {
            // Cập nhật thông tin từ AuthServer
            user.UserName = currentUserName;
            user.Email = currentEmail;
            user.LastLoginTime = Clock.Now;
            
            user = await _userBaseRepository.UpdateAsync(user, autoSave: true);
        }

        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Cập nhật avatar cho user hiện tại
    /// </summary>
    public virtual async Task<SmartBoxUserDto> UpdateAvatarAsync(string avatar)
    {
        var currentUserId = CurrentUser.GetId();
        var user = await _userRepository.FindByAuthUserIdAsync(currentUserId);
        
        if (user == null)
        {
            throw new BusinessException("USER_NOT_FOUND", "Không tìm thấy thông tin người dùng hiện tại");
        }

        user.Avatar = avatar;
        user = await _userBaseRepository.UpdateAsync(user, autoSave: true);
        
        return ObjectMapper.Map<SmartBoxUser, SmartBoxUserDto>(user);
    }

    /// <summary>
    /// Helper method để cập nhật roles của user
    /// </summary>
    private async Task UpdateUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        // Xóa tất cả roles hiện tại
        var currentUserRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        await _userRoleRepository.DeleteManyAsync(currentUserRoles);

        // Thêm roles mới
        if (roleIds.Any())
        {
            var newUserRoles = roleIds.Select(roleId => new SmartBoxUserRole(
                userId,
                roleId,
                CurrentUser.Id,
                CurrentTenant.Id
            )).ToList();

            await _userRoleRepository.InsertManyAsync(newUserRoles, autoSave: true);
        }
    }
} 