using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service interface cho quản lý người dùng SmartBox
/// </summary>
public interface ISmartBoxUserAppService : IApplicationService
{
    /// <summary>
    /// Lấy danh sách người dùng với phân trang
    /// </summary>
    Task<PagedResultDto<SmartBoxUserDto>> GetListAsync(GetSmartBoxUsersInput input);

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    Task<SmartBoxUserDto> GetAsync(Guid id);

    /// <summary>
    /// Lấy thông tin người dùng hiện tại (từ JWT claims)
    /// </summary>
    Task<SmartBoxUserDto> GetCurrentUserAsync();

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    Task<SmartBoxUserDto> UpdateAsync(Guid id, CreateUpdateSmartBoxUserDto input);

    /// <summary>
    /// Cập nhật thông tin cá nhân của user hiện tại
    /// </summary>
    Task<SmartBoxUserDto> UpdateCurrentUserAsync(CreateUpdateSmartBoxUserDto input);

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Kích hoạt/Vô hiệu hóa người dùng
    /// </summary>
    Task<SmartBoxUserDto> SetActiveAsync(Guid id, bool isActive);

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds);

    /// <summary>
    /// Lấy danh sách người dùng theo công ty
    /// </summary>
    Task<PagedResultDto<SmartBoxUserDto>> GetUsersByCompanyAsync(Guid companyId, GetSmartBoxUsersInput input);

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò
    /// </summary>
    Task<PagedResultDto<SmartBoxUserDto>> GetUsersByRoleAsync(Guid roleId, GetSmartBoxUsersInput input);

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    Task<bool> IsUserNameExistAsync(string userName, Guid? excludeId = null);

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmailExistAsync(string email, Guid? excludeId = null);

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmployeeCodeExistAsync(string employeeCode, Guid? excludeId = null);

    /// <summary>
    /// Đồng bộ thông tin user từ AuthServer (được gọi khi user đăng nhập)
    /// </summary>
    Task<SmartBoxUserDto> SyncCurrentUserAsync();

    /// <summary>
    /// Cập nhật avatar cho user hiện tại
    /// </summary>
    Task<SmartBoxUserDto> UpdateAvatarAsync(string avatar);
}

/// <summary>
/// Input DTO cho lấy danh sách người dùng
/// </summary>
public class GetSmartBoxUsersInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Từ khóa tìm kiếm (tên, email, mã nhân viên)
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Chỉ lấy người dùng hoạt động
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Lọc theo công ty
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Lọc theo vai trò
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// Lọc theo phòng ban
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Lọc theo chức vụ
    /// </summary>
    public string? Position { get; set; }
} 