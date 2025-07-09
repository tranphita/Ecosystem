using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Repositories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecosystem.SmartBox.Users;

/// <summary>
/// Application Service implementation cho quản lý công ty
/// </summary>
[Authorize(SmartBoxPermissions.Companies.Default)]
public class CompanyAppService : ApplicationService, ICompanyAppService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyAppService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public virtual async Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input)
    {
        var filter = input.Filter?.Trim();
        var sorting = string.IsNullOrEmpty(input.Sorting) ? "Name" : input.Sorting;

        var query = await _companyRepository.GetQueryableAsync();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => 
                x.Name.Contains(filter) || 
                x.TaxCode.Contains(filter) || 
                x.Email.Contains(filter));
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == input.IsActive.Value);
        }

        // Apply sorting and paging
        var totalCount = query.Count();
        
        // Apply manual sorting since OrderBy(string) is not available in LINQ to Objects
        if (sorting.ToLowerInvariant().Contains("desc"))
        {
            query = query.OrderByDescending(x => x.Name);
        }
        else
        {
            query = query.OrderBy(x => x.Name);
        }
        
        var companies = await AsyncExecuter.ToListAsync(
            query.Skip(input.SkipCount)
                .Take(input.MaxResultCount));

        var companyDtos = ObjectMapper.Map<List<Company>, List<CompanyDto>>(companies);

        return new PagedResultDto<CompanyDto>(totalCount, companyDtos);
    }

    public virtual async Task<CompanyDto> GetAsync(Guid id)
    {
        var company = await _companyRepository.GetAsync(id);
        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    [Authorize(SmartBoxPermissions.Companies.Create)]
    public virtual async Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto input)
    {
        // Kiểm tra tên công ty đã tồn tại
        if (await _companyRepository.IsNameExistAsync(input.Name))
        {
            throw new UserFriendlyException(L["CompanyNameAlreadyExists", input.Name]);
        }

        // Kiểm tra mã số thuế đã tồn tại (nếu có)
        if (!string.IsNullOrEmpty(input.TaxCode) && 
            await _companyRepository.IsTaxCodeExistAsync(input.TaxCode))
        {
            throw new UserFriendlyException(L["CompanyTaxCodeAlreadyExists", input.TaxCode]);
        }

        var company = new Company(
            GuidGenerator.Create(),
            input.Name,
            input.TaxCode,
            input.Address,
            input.PhoneNumber,
            input.Email,
            CurrentTenant.Id
        );

        // Map remaining properties
        ObjectMapper.Map(input, company);
        
        await _companyRepository.InsertAsync(company, autoSave: true);
        
        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    [Authorize(SmartBoxPermissions.Companies.Edit)]
    public virtual async Task<CompanyDto> UpdateAsync(Guid id, CreateUpdateCompanyDto input)
    {
        var company = await _companyRepository.GetAsync(id);

        // Kiểm tra tên công ty đã tồn tại (trừ chính nó)
        if (await _companyRepository.IsNameExistAsync(input.Name, id))
        {
            throw new UserFriendlyException(L["CompanyNameAlreadyExists", input.Name]);
        }

        // Kiểm tra mã số thuế đã tồn tại (nếu có và trừ chính nó)
        if (!string.IsNullOrEmpty(input.TaxCode) && 
            await _companyRepository.IsTaxCodeExistAsync(input.TaxCode, id))
        {
            throw new UserFriendlyException(L["CompanyTaxCodeAlreadyExists", input.TaxCode]);
        }

        ObjectMapper.Map(input, company);
        
        await _companyRepository.UpdateAsync(company, autoSave: true);
        
        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    [Authorize(SmartBoxPermissions.Companies.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var company = await _companyRepository.GetAsync(id);
        
        // TODO: Kiểm tra xem có user nào đang thuộc công ty này không
        // var hasUsers = await _userRepository.GetCountAsync(x => x.CompanyId == id);
        // if (hasUsers > 0)
        // {
        //     throw new UserFriendlyException(L["CompanyHasUsersCannotDelete"]);
        // }

        await _companyRepository.DeleteAsync(company, autoSave: true);
    }

    public virtual async Task<ListResultDto<CompanyDto>> GetActiveCompaniesAsync()
    {
        var companies = await _companyRepository.GetActiveCompaniesAsync(
            sorting: "Name",
            includeDetails: false
        );

        var companyDtos = ObjectMapper.Map<List<Company>, List<CompanyDto>>(companies);
        
        return new ListResultDto<CompanyDto>(companyDtos);
    }

    public virtual Task<bool> IsNameExistAsync(string name, Guid? excludeId = null)
    {
        return _companyRepository.IsNameExistAsync(name, excludeId);
    }

    public virtual Task<bool> IsTaxCodeExistAsync(string taxCode, Guid? excludeId = null)
    {
        return _companyRepository.IsTaxCodeExistAsync(taxCode, excludeId);
    }
} 