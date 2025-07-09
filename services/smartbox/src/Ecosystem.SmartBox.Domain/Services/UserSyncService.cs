using System;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Repositories;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Ecosystem.SmartBox.Services;

/// <summary>
/// Service implementation cho việc đồng bộ user từ AuthServer
/// </summary>
public class UserSyncService : DomainService, IUserSyncService
{
    private readonly ISmartBoxUserRepository _userRepository;
    private readonly ILogger<UserSyncService> _logger;

    public UserSyncService(
        ISmartBoxUserRepository userRepository,
        ILogger<UserSyncService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task<SmartBoxUser> SyncUserAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Đồng bộ user {AuthUserId} - {UserName}", authUserId, userName);

            var existingUser = await _userRepository.FindByAuthUserIdAsync(
                authUserId, 
                includeDetails: false, 
                cancellationToken
            );

            if (existingUser != null)
            {
                // Cập nhật thông tin user hiện tại
                existingUser.UpdateBasicInfo(userName, email, fullName);
                existingUser.UpdateLastLoginTime();
                
                await _userRepository.UpdateAsync(existingUser, autoSave: true, cancellationToken);
                
                _logger.LogInformation("Đã cập nhật thông tin user {AuthUserId}", authUserId);
                return existingUser;
            }
            else
            {
                // Tạo user mới
                var newUser = new SmartBoxUser(
                    GuidGenerator.Create(),
                    authUserId,
                    userName,
                    email,
                    fullName,
                    tenantId: CurrentTenant.Id
                );
                
                newUser.UpdateLastLoginTime();
                
                await _userRepository.InsertAsync(newUser, autoSave: true, cancellationToken);
                
                _logger.LogInformation("Đã tạo user mới {AuthUserId} - {UserName}", authUserId, userName);
                return newUser;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đồng bộ user {AuthUserId}", authUserId);
            throw;
        }
    }

    [UnitOfWork]
    public virtual async Task UpdateLastLoginTimeAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.FindByAuthUserIdAsync(
                authUserId, 
                includeDetails: false, 
                cancellationToken
            );

            if (user != null)
            {
                user.UpdateLastLoginTime();
                await _userRepository.UpdateAsync(user, autoSave: true, cancellationToken);
                
                _logger.LogDebug("Đã cập nhật thời gian đăng nhập cuối cho user {AuthUserId}", authUserId);
            }
            else
            {
                _logger.LogWarning("Không tìm thấy user với AuthUserId {AuthUserId}", authUserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật thời gian đăng nhập cho user {AuthUserId}", authUserId);
            throw;
        }
    }

    public virtual async Task<SmartBoxUser> GetOrCreateUserAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.FindByAuthUserIdAsync(
            authUserId, 
            includeDetails: true, 
            cancellationToken
        );

        if (existingUser != null)
        {
            return existingUser;
        }

        return await SyncUserAsync(authUserId, userName, email, fullName, cancellationToken);
    }

    public virtual async Task<bool> IsUserExistAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByAuthUserIdAsync(
            authUserId, 
            includeDetails: false, 
            cancellationToken
        );

        return user != null;
    }

    [UnitOfWork]
    public virtual async Task UpdateUserBasicInfoAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.FindByAuthUserIdAsync(
                authUserId, 
                includeDetails: false, 
                cancellationToken
            );

            if (user != null)
            {
                user.UpdateBasicInfo(userName, email, fullName);
                await _userRepository.UpdateAsync(user, autoSave: true, cancellationToken);
                
                _logger.LogInformation("Đã cập nhật thông tin cơ bản cho user {AuthUserId}", authUserId);
            }
            else
            {
                _logger.LogWarning("Không tìm thấy user với AuthUserId {AuthUserId}", authUserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật thông tin cơ bản cho user {AuthUserId}", authUserId);
            throw;
        }
    }
} 