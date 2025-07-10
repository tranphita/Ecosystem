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
/// Repository implementation cho entity SmartBoxRole
/// </summary>
public class SmartBoxRoleRepository : EfCoreRepository<SmartBoxDbContext, SmartBoxRole, Guid>, ISmartBoxRoleRepository
{
    public SmartBoxRoleRepository(IDbContextProvider<SmartBoxDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Tìm role theo tên
    /// </summary>
    public async Task<SmartBoxRole?> FindByNameAsync(
        string name,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách role hoạt động
    /// </summary>
    public async Task<List<SmartBoxRole>> GetActiveRolesAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "DisplayOrder,DisplayName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                x.Name.Contains(filter) ||
                x.DisplayName.Contains(filter) ||
                (x.Description != null && x.Description.Contains(filter)));
        }

        query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.DisplayName);

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách role hệ thống
    /// </summary>
    public async Task<List<SmartBoxRole>> GetSystemRolesAsync(
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.IsSystem)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.DisplayName)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách role của user
    /// </summary>
    public async Task<List<SmartBoxRole>> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        
        return await (from role in dbContext.SmartBoxRoles
                     join userRole in dbContext.SmartBoxUserRoles on role.Id equals userRole.RoleId
                     where userRole.UserId == userId && role.IsActive
                     orderby role.DisplayOrder, role.DisplayName
                     select role)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Đếm số lượng role hoạt động
    /// </summary>
    public async Task<long> GetActiveRolesCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x =>
                x.Name.Contains(filter) ||
                x.DisplayName.Contains(filter) ||
                (x.Description != null && x.Description.Contains(filter)));
        }

        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Kiểm tra tên role đã tồn tại chưa
    /// </summary>
    public async Task<bool> IsNameExistAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    public async Task<bool> IsDisplayNameExistAsync(
        string displayName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync()).Where(x => x.DisplayName == displayName);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }
} 