using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service interface cho quản lý vai trò SmartBox
/// </summary>
public interface ISmartBoxRoleAppService : IApplicationService
{
    /// <summary>
    /// Lấy danh sách vai trò với phân trang
    /// </summary>
    Task<PagedResultDto<SmartBoxRoleDto>> GetListAsync(GetSmartBoxRolesInput input);

    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    Task<SmartBoxRoleDto> GetAsync(Guid id);

    /// <summary>
    /// Tạo vai trò mới
    /// </summary>
    Task<SmartBoxRoleDto> CreateAsync(CreateUpdateSmartBoxRoleDto input);

    /// <summary>
    /// Cập nhật thông tin vai trò
    /// </summary>
    Task<SmartBoxRoleDto> UpdateAsync(Guid id, CreateUpdateSmartBoxRoleDto input);

    /// <summary>
    /// Xóa vai trò (chỉ có thể xóa role không phải hệ thống)
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Lấy danh sách vai trò hoạt động cho dropdown
    /// </summary>
    Task<ListResultDto<SmartBoxRoleDto>> GetActiveRolesAsync();

    /// <summary>
    /// Lấy danh sách vai trò của user
    /// </summary>
    Task<ListResultDto<SmartBoxRoleDto>> GetRolesByUserIdAsync(Guid userId);

    /// <summary>
    /// Kiểm tra tên vai trò đã tồn tại chưa
    /// </summary>
    Task<bool> IsNameExistAsync(string name, Guid? excludeId = null);

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    Task<bool> IsDisplayNameExistAsync(string displayName, Guid? excludeId = null);
}

/// <summary>
/// Input DTO cho lấy danh sách vai trò
/// </summary>
public class GetSmartBoxRolesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Từ khóa tìm kiếm
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Chỉ lấy vai trò hoạt động
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Chỉ lấy vai trò hệ thống
    /// </summary>
    public bool? IsSystem { get; set; }
} 