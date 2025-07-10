using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Ecosystem.SmartBox.Repositories;

/// <summary>
/// Repository implementation cho entity SmartBoxUser
/// </summary>
public class SmartBoxUserRepository : EfCoreRepository<SmartBoxDbContext, SmartBoxUser, Guid>, ISmartBoxUserRepository
{
    public SmartBoxUserRepository(IDbContextProvider<SmartBoxDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Tìm user theo AuthUserId
    /// </summary>
    public async Task<SmartBoxUser?> FindByAuthUserIdAsync(
        Guid authUserId,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.AuthUserId == authUserId, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Tìm user theo tên đăng nhập
    /// </summary>
    public async Task<SmartBoxUser?> FindByUserNameAsync(
        string userName,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.UserName == userName, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Tìm user theo email
    /// </summary>
    public async Task<SmartBoxUser?> FindByEmailAsync(
        string email,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.Email == email, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Tìm user theo mã nhân viên
    /// </summary>
    public async Task<SmartBoxUser?> FindByEmployeeCodeAsync(
        string employeeCode,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách user theo công ty
    /// </summary>
    public async Task<List<SmartBoxUser>> GetUsersByCompanyAsync(
        Guid companyId,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(filter)) ||
                x.UserName.Contains(filter) ||
                x.Email.Contains(filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(filter)));
        }

        query = query.OrderBy(x => x.FullName);

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách user có vai trò cụ thể
    /// </summary>
    public async Task<List<SmartBoxUser>> GetUsersByRoleAsync(
        Guid roleId,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        
        var query = from user in dbContext.SmartBoxUsers
                   join userRole in dbContext.SmartBoxUserRoles on user.Id equals userRole.UserId
                   where userRole.RoleId == roleId
                   select user;

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(filter)) ||
                x.UserName.Contains(filter) ||
                x.Email.Contains(filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(filter)));
        }

        query = query.OrderBy(x => x.FullName);

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách user hoạt động
    /// </summary>
    public async Task<List<SmartBoxUser>> GetActiveUsersAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(filter)) ||
                x.UserName.Contains(filter) ||
                x.Email.Contains(filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(filter)));
        }

        query = query.OrderBy(x => x.FullName);

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Đếm số lượng user hoạt động
    /// </summary>
    public async Task<long> GetActiveUsersCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(filter)) ||
                x.UserName.Contains(filter) ||
                x.Email.Contains(filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(filter)));
        }

        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    public async Task<bool> IsUserNameExistAsync(
        string userName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.UserName == userName);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    public async Task<bool> IsEmailExistAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Kiểm tra mã nhân viên đã tồn tại chưa
    /// </summary>
    public async Task<bool> IsEmployeeCodeExistAsync(
        string employeeCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.EmployeeCode == employeeCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy user với roles và company
    /// </summary>
    public async Task<SmartBoxUser?> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.Id == id, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách user với roles
    /// </summary>
    public async Task<List<SmartBoxUser>> GetListWithRolesAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "FullName",
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(filter)) ||
                x.UserName.Contains(filter) ||
                x.Email.Contains(filter) ||
                (x.EmployeeCode != null && x.EmployeeCode.Contains(filter)));
        }

        query = query.OrderBy(x => x.FullName);

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
} 