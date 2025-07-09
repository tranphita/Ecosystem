using System;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.SmartBox.Controllers;

/// <summary>
/// API Controller cho quản lý công ty
/// </summary>
[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/smartbox/companies")]
public class CompanyController : SmartBoxController, ICompanyAppService
{
    private readonly ICompanyAppService _companyAppService;

    public CompanyController(ICompanyAppService companyAppService)
    {
        _companyAppService = companyAppService;
    }

    /// <summary>
    /// Lấy danh sách công ty với phân trang
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input)
    {
        return _companyAppService.GetListAsync(input);
    }

    /// <summary>
    /// Lấy thông tin công ty theo ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual Task<CompanyDto> GetAsync(Guid id)
    {
        return _companyAppService.GetAsync(id);
    }

    /// <summary>
    /// Tạo công ty mới
    /// </summary>
    [HttpPost]
    public virtual Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto input)
    {
        return _companyAppService.CreateAsync(input);
    }

    /// <summary>
    /// Cập nhật thông tin công ty
    /// </summary>
    [HttpPut("{id}")]
    public virtual Task<CompanyDto> UpdateAsync(Guid id, CreateUpdateCompanyDto input)
    {
        return _companyAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Xóa công ty
    /// </summary>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _companyAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Lấy danh sách công ty hoạt động cho dropdown
    /// </summary>
    [HttpGet("active")]
    public virtual Task<ListResultDto<CompanyDto>> GetActiveCompaniesAsync()
    {
        return _companyAppService.GetActiveCompaniesAsync();
    }

    /// <summary>
    /// Kiểm tra tên công ty đã tồn tại chưa
    /// </summary>
    [HttpGet("check-name")]
    public virtual Task<bool> IsNameExistAsync(string name, Guid? excludeId = null)
    {
        return _companyAppService.IsNameExistAsync(name, excludeId);
    }

    /// <summary>
    /// Kiểm tra mã số thuế đã tồn tại chưa
    /// </summary>
    [HttpGet("check-tax-code")]
    public virtual Task<bool> IsTaxCodeExistAsync(string taxCode, Guid? excludeId = null)
    {
        return _companyAppService.IsTaxCodeExistAsync(taxCode, excludeId);
    }
} 