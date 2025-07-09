using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Volo.Abp.Domain.Repositories;

namespace Ecosystem.SmartBox.Repositories;

/// <summary>
/// Repository interface cho entity Company
/// </summary>
public interface ICompanyRepository : IRepository<Company, Guid>
{
    /// <summary>
    /// Tìm công ty theo mã số thuế
    /// </summary>
    Task<Company?> FindByTaxCodeAsync(
        string taxCode,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách công ty hoạt động
    /// </summary>
    Task<List<Company>> GetActiveCompaniesAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "Name",
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Đếm số lượng công ty hoạt động
    /// </summary>
    Task<long> GetActiveCompaniesCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra tên công ty đã tồn tại chưa
    /// </summary>
    Task<bool> IsNameExistAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra mã số thuế đã tồn tại chưa
    /// </summary>
    Task<bool> IsTaxCodeExistAsync(
        string taxCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
} 