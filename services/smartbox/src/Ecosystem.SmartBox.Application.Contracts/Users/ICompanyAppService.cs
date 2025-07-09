using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service interface cho quản lý công ty
/// </summary>
public interface ICompanyAppService : IApplicationService
{
    /// <summary>
    /// Lấy danh sách công ty với phân trang
    /// </summary>
    Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input);

    /// <summary>
    /// Lấy thông tin công ty theo ID
    /// </summary>
    Task<CompanyDto> GetAsync(Guid id);

    /// <summary>
    /// Tạo công ty mới
    /// </summary>
    Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto input);

    /// <summary>
    /// Cập nhật thông tin công ty
    /// </summary>
    Task<CompanyDto> UpdateAsync(Guid id, CreateUpdateCompanyDto input);

    /// <summary>
    /// Xóa công ty
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Lấy danh sách công ty hoạt động cho dropdown
    /// </summary>
    Task<ListResultDto<CompanyDto>> GetActiveCompaniesAsync();

    /// <summary>
    /// Kiểm tra tên công ty đã tồn tại chưa
    /// </summary>
    Task<bool> IsNameExistAsync(string name, Guid? excludeId = null);

    /// <summary>
    /// Kiểm tra mã số thuế đã tồn tại chưa
    /// </summary>
    Task<bool> IsTaxCodeExistAsync(string taxCode, Guid? excludeId = null);
}

/// <summary>
/// Input DTO cho lấy danh sách công ty
/// </summary>
public class GetCompaniesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Từ khóa tìm kiếm
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Chỉ lấy công ty hoạt động
    /// </summary>
    public bool? IsActive { get; set; }
} 