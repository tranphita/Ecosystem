using System.Threading.Tasks;
using Ecosystem.Administration.OpenIddict;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Controller cho quản lý OpenIddict Applications
/// </summary>
[Area("administration")]
[Route("api/openiddict/applications")]
public class OpenIddictApplicationController : AdministrationController
{
    private readonly IOpenIddictApplicationAppService _applicationAppService;

    public OpenIddictApplicationController(IOpenIddictApplicationAppService applicationAppService)
    {
        _applicationAppService = applicationAppService;
    }

    /// <summary>
    /// Lấy danh sách applications với phân trang và tìm kiếm
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationListDto input)
    {
        return _applicationAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một application theo ID
    /// </summary>
    [HttpGet("{id}")]
    public Task<OpenIddictApplicationDto> GetAsync(string id)
    {
        return _applicationAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo mới một application
    /// </summary>
    [HttpPost]
    public Task<OpenIddictApplicationDto> CreateAsync(CreateOpenIddictApplicationDto input)
    {
        return _applicationAppService.CreateAsync(input);
    }

    /// <summary>
    /// Cập nhật thông tin application
    /// </summary>
    [HttpPut("{id}")]
    public Task<OpenIddictApplicationDto> UpdateAsync(string id, UpdateOpenIddictApplicationDto input)
    {
        return _applicationAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Xóa application theo ID
    /// </summary>
    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _applicationAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra Client ID đã tồn tại hay chưa
    /// </summary>
    [HttpGet("check-client-id")]
    public Task<bool> IsClientIdExistsAsync(string clientId)
    {
        return _applicationAppService.IsClientIdExistsAsync(clientId);
    }

    /// <summary>
    /// Lấy danh sách permissions có sẵn cho applications
    /// </summary>
    [HttpGet("available-permissions")]
    public Task<string[]> GetAvailablePermissionsAsync()
    {
        return _applicationAppService.GetAvailablePermissionsAsync();
    }

    /// <summary>
    /// Lấy danh sách scopes có sẵn cho applications
    /// </summary>
    [HttpGet("available-scopes")]
    public Task<string[]> GetAvailableScopesAsync()
    {
        return _applicationAppService.GetAvailableScopesAsync();
    }
} 