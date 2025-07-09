using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Volo.Abp.Domain.Repositories;

namespace Ecosystem.SmartBox.Repositories;

/// <summary>
/// Repository interface cho entity SmartBoxRole
/// </summary>
public interface ISmartBoxRoleRepository : IRepository<SmartBoxRole, Guid>
{
    /// <summary>
    /// Tìm role theo tên
    /// </summary>
    Task<SmartBoxRole?> FindByNameAsync(
        string name,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách role hoạt động
    /// </summary>
    Task<List<SmartBoxRole>> GetActiveRolesAsync(
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "DisplayOrder,DisplayName",
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách role hệ thống
    /// </summary>
    Task<List<SmartBoxRole>> GetSystemRolesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lấy danh sách role của user
    /// </summary>
    Task<List<SmartBoxRole>> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Đếm số lượng role hoạt động
    /// </summary>
    Task<long> GetActiveRolesCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra tên role đã tồn tại chưa
    /// </summary>
    Task<bool> IsNameExistAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra tên hiển thị đã tồn tại chưa
    /// </summary>
    Task<bool> IsDisplayNameExistAsync(
        string displayName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
} 