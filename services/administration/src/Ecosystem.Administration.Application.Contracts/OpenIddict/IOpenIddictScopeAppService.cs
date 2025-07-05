using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Interface cho Application Service quản lý OpenIddict Scopes
/// </summary>
public interface IOpenIddictScopeAppService : IApplicationService
{
    /// <summary>
    /// Lấy danh sách scopes với phân trang và tìm kiếm
    /// </summary>
    /// <param name="input">Tham số lọc và phân trang</param>
    /// <returns>Danh sách scopes</returns>
    Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopeListDto input);

    /// <summary>
    /// Lấy thông tin chi tiết một scope theo ID
    /// </summary>
    /// <param name="id">ID của scope</param>
    /// <returns>Thông tin chi tiết scope</returns>
    Task<OpenIddictScopeDto> GetAsync(string id);

    /// <summary>
    /// Tạo mới một scope
    /// </summary>
    /// <param name="input">Thông tin scope cần tạo</param>
    /// <returns>Scope vừa được tạo</returns>
    Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input);

    /// <summary>
    /// Cập nhật thông tin scope
    /// </summary>
    /// <param name="id">ID của scope cần cập nhật</param>
    /// <param name="input">Thông tin cập nhật</param>
    /// <returns>Scope sau khi cập nhật</returns>
    Task<OpenIddictScopeDto> UpdateAsync(string id, UpdateOpenIddictScopeDto input);

    /// <summary>
    /// Xóa scope theo ID
    /// </summary>
    /// <param name="id">ID của scope cần xóa</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Kiểm tra tên scope đã tồn tại hay chưa
    /// </summary>
    /// <param name="name">Tên scope cần kiểm tra</param>
    /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
    Task<bool> IsScopeNameExistsAsync(string name);

    /// <summary>
    /// Lấy danh sách resources có sẵn cho scopes
    /// </summary>
    /// <returns>Danh sách resources</returns>
    Task<string[]> GetAvailableResourcesAsync();
} 