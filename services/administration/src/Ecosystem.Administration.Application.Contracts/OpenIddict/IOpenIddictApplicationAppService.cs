using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Interface cho Application Service quản lý OpenIddict Applications
/// </summary>
public interface IOpenIddictApplicationAppService : IApplicationService
{
    /// <summary>
    /// Lấy danh sách applications với phân trang và tìm kiếm
    /// </summary>
    /// <param name="input">Tham số lọc và phân trang</param>
    /// <returns>Danh sách applications</returns>
    Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationListDto input);

    /// <summary>
    /// Lấy thông tin chi tiết một application theo ID
    /// </summary>
    /// <param name="id">ID của application</param>
    /// <returns>Thông tin chi tiết application</returns>
    Task<OpenIddictApplicationDto> GetAsync(string id);

    /// <summary>
    /// Tạo mới một application
    /// </summary>
    /// <param name="input">Thông tin application cần tạo</param>
    /// <returns>Application vừa được tạo</returns>
    Task<OpenIddictApplicationDto> CreateAsync(CreateOpenIddictApplicationDto input);

    /// <summary>
    /// Cập nhật thông tin application
    /// </summary>
    /// <param name="id">ID của application cần cập nhật</param>
    /// <param name="input">Thông tin cập nhật</param>
    /// <returns>Application sau khi cập nhật</returns>
    Task<OpenIddictApplicationDto> UpdateAsync(string id, UpdateOpenIddictApplicationDto input);

    /// <summary>
    /// Xóa application theo ID
    /// </summary>
    /// <param name="id">ID của application cần xóa</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Kiểm tra Client ID đã tồn tại hay chưa
    /// </summary>
    /// <param name="clientId">Client ID cần kiểm tra</param>
    /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
    Task<bool> IsClientIdExistsAsync(string clientId);

    /// <summary>
    /// Lấy danh sách permissions có sẵn cho applications
    /// </summary>
    /// <returns>Danh sách permissions</returns>
    Task<string[]> GetAvailablePermissionsAsync();

    /// <summary>
    /// Lấy danh sách scopes có sẵn cho applications
    /// </summary>
    /// <returns>Danh sách scopes</returns>
    Task<string[]> GetAvailableScopesAsync();
} 