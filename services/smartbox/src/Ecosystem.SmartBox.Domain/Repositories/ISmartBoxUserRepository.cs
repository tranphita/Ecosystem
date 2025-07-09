using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Volo.Abp.Domain.Repositories;

namespace Ecosystem.SmartBox.Repositories;

/// <summary>
/// Repository interface cho entity SmartBoxUser
/// </summary>
public interface ISmartBoxUserRepository : IRepository<SmartBoxUser, Guid>
{
    /// <summary>
    /// Tìm user theo AuthUserId
    /// </summary>
    Task<SmartBoxUser?> FindByAuthUserIdAsync(
        Guid authUserId,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Tìm user theo tên đăng nhập
    /// </summary>
    Task<SmartBoxUser?> FindByUserNameAsync(
        string userName,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Tìm user theo email
    /// </summary>
    Task<SmartBoxUser?> FindByEmailAsync(
        string email,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Tìm user theo mã nhân viên
    /// </summary>
    Task<SmartBoxUser?> FindByEmployeeCodeAsync(
        string employeeCode,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách user theo công ty
    /// </summary>
    Task<List<SmartBoxUser>> GetUsersByCompanyAsync(
        Guid companyId,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách user có vai trò cụ thể
    /// </summary>
    Task<List<SmartBoxUser>> GetUsersByRoleAsync(
        Guid roleId,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách user hoạt động
    /// </summary>
    Task<List<SmartBoxUser>> GetActiveUsersAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Đếm số lượng user hoạt động
    /// </summary>
    Task<long> GetActiveUsersCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    Task<bool> IsUserNameExistAsync(
        string userName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmailExistAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmployeeCodeExistAsync(
        string employeeCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy user với roles và company
    /// </summary>
    Task<SmartBoxUser?> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách user với roles
    /// </summary>
    Task<List<SmartBoxUser>> GetListWithRolesAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        CancellationToken cancellationToken = default
    );
} 