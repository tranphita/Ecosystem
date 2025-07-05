using System.Threading.Tasks;
using Ecosystem.Administration.OpenIddict;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.Administration.OpenIddict;

/// <summary>
/// Controller cho quản lý OpenIddict Scopes
/// </summary>
[Area("administration")]
[Route("api/openiddict/scopes")]
public class OpenIddictScopeController : AdministrationController
{
    private readonly IOpenIddictScopeAppService _scopeAppService;

    public OpenIddictScopeController(IOpenIddictScopeAppService scopeAppService)
    {
        _scopeAppService = scopeAppService;
    }

    /// <summary>
    /// Lấy danh sách scopes với phân trang và tìm kiếm
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopeListDto input)
    {
        return _scopeAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một scope theo ID
    /// </summary>
    [HttpGet("{id}")]
    public Task<OpenIddictScopeDto> GetAsync(string id)
    {
        return _scopeAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo mới một scope
    /// </summary>
    [HttpPost]
    public Task<OpenIddictScopeDto> CreateAsync(CreateOpenIddictScopeDto input)
    {
        return _scopeAppService.CreateAsync(input);
    }

    /// <summary>
    /// Cập nhật thông tin scope
    /// </summary>
    [HttpPut("{id}")]
    public Task<OpenIddictScopeDto> UpdateAsync(string id, UpdateOpenIddictScopeDto input)
    {
        return _scopeAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Xóa scope theo ID
    /// </summary>
    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _scopeAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra tên scope đã tồn tại hay chưa
    /// </summary>
    [HttpGet("check-scope-name")]
    public Task<bool> IsScopeNameExistsAsync(string name)
    {
        return _scopeAppService.IsScopeNameExistsAsync(name);
    }

    /// <summary>
    /// Lấy danh sách resources có sẵn cho scopes
    /// </summary>
    [HttpGet("available-resources")]
    public Task<string[]> GetAvailableResourcesAsync()
    {
        return _scopeAppService.GetAvailableResourcesAsync();
    }
} 