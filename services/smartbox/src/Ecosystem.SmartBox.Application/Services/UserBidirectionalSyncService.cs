using System;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Repositories;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Ecosystem.SmartBox.Services;

/// <summary>
/// Implementation th?c t? cho vi?c ??ng b? user 2 chi?u v?i Identity Service
/// </summary>
public class UserBidirectionalSyncService : DomainService, IUserBidirectionalSyncService
{
    private readonly ISmartBoxUserRepository _userRepository;
    private readonly IIdentityUserAppService _identityUserAppService;
    private readonly IDistributedEventBus _distributedEventBus;
    private readonly ILogger<UserBidirectionalSyncService> _logger;

    public UserBidirectionalSyncService(
        ISmartBoxUserRepository userRepository,
        IIdentityUserAppService identityUserAppService,
        IDistributedEventBus distributedEventBus,
        ILogger<UserBidirectionalSyncService> logger)
    {
        _userRepository = userRepository;
        _identityUserAppService = identityUserAppService;
        _distributedEventBus = distributedEventBus;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task<(Guid AuthUserId, SmartBoxUser SmartBoxUser)> CreateUserInIdentityAsync(
        string userName,
        string email,
        string password,
        string? fullName = null,
        string? phoneNumber = null,
        bool isActive = true,
        bool requirePasswordChange = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("T?o user trong Identity Service: {UserName} - {Email}", userName, email);

            // T?o user trong Identity Service
            var createUserDto = new IdentityUserCreateDto
            {
                UserName = userName,
                Email = email,
                Password = password,
                Name = fullName,
                PhoneNumber = phoneNumber,
                IsActive = isActive,
                LockoutEnabled = false
                // Note: Removed ShouldChangePasswordOnNextLogin as it's not available in this version
            };

            var identityUser = await _identityUserAppService.CreateAsync(createUserDto);
            var authUserId = identityUser.Id;

            _logger.LogInformation("?ã t?o user trong Identity Service: {AuthUserId}", authUserId);

            // T?o user trong SmartBox
            var smartBoxUser = new SmartBoxUser(
                GuidGenerator.Create(),
                authUserId,
                userName,
                email,
                fullName,
                tenantId: CurrentTenant.Id
            );

            smartBoxUser.PhoneNumber = phoneNumber;
            smartBoxUser.IsActive = isActive;

            await _userRepository.InsertAsync(smartBoxUser, autoSave: true, cancellationToken);

            _logger.LogInformation("?ã t?o user trong SmartBox: {SmartBoxUserId}", smartBoxUser.Id);

            return (authUserId, smartBoxUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi t?o user trong Identity Service: {UserName}", userName);
            throw;
        }
    }

    [UnitOfWork]
    public virtual async Task SyncSmartBoxUserToIdentityAsync(
        SmartBoxUser smartBoxUser,
        bool syncBasicInfo = true,
        bool syncStatus = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("??ng b? user t? SmartBox sang Identity: {AuthUserId}", smartBoxUser.AuthUserId);

            // L?y thông tin user hi?n t?i t? Identity Service
            var identityUser = await _identityUserAppService.GetAsync(smartBoxUser.AuthUserId);

            var updateDto = new IdentityUserUpdateDto
            {
                UserName = smartBoxUser.UserName,
                Email = smartBoxUser.Email,
                ConcurrencyStamp = identityUser.ConcurrencyStamp
            };

            if (syncBasicInfo)
            {
                updateDto.Name = smartBoxUser.FullName;
                updateDto.PhoneNumber = smartBoxUser.PhoneNumber;
            }

            if (syncStatus)
            {
                updateDto.IsActive = smartBoxUser.IsActive;
            }

            await _identityUserAppService.UpdateAsync(smartBoxUser.AuthUserId, updateDto);

            _logger.LogInformation("?ã ??ng b? user sang Identity Service: {AuthUserId}", smartBoxUser.AuthUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi ??ng b? user sang Identity Service: {AuthUserId}", smartBoxUser.AuthUserId);
            throw;
        }
    }

    [UnitOfWork]
    public virtual async Task<SmartBoxUser> SyncIdentityUserToSmartBoxAsync(
        Guid authUserId,
        string userName,
        string email,
        string? fullName = null,
        string? phoneNumber = null,
        bool isActive = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("??ng b? user t? Identity sang SmartBox: {AuthUserId}", authUserId);

            var existingUser = await _userRepository.FindByAuthUserIdAsync(authUserId, includeDetails: false, cancellationToken);

            if (existingUser != null)
            {
                // C?p nh?t user hi?n có
                existingUser.UpdateBasicInfo(userName, email, fullName);
                existingUser.PhoneNumber = phoneNumber;
                existingUser.IsActive = isActive;

                await _userRepository.UpdateAsync(existingUser, autoSave: true, cancellationToken);

                _logger.LogInformation("?ã c?p nh?t user trong SmartBox: {SmartBoxUserId}", existingUser.Id);
                return existingUser;
            }
            else
            {
                // T?o user m?i
                var newUser = new SmartBoxUser(
                    GuidGenerator.Create(),
                    authUserId,
                    userName,
                    email,
                    fullName,
                    tenantId: CurrentTenant.Id
                );

                newUser.PhoneNumber = phoneNumber;
                newUser.IsActive = isActive;

                await _userRepository.InsertAsync(newUser, autoSave: true, cancellationToken);

                _logger.LogInformation("?ã t?o user m?i trong SmartBox: {SmartBoxUserId}", newUser.Id);
                return newUser;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi ??ng b? user t? Identity sang SmartBox: {AuthUserId}", authUserId);
            throw;
        }
    }

    public virtual async Task DeleteUserFromIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Xóa user trong Identity Service: {AuthUserId}", authUserId);

            await _identityUserAppService.DeleteAsync(authUserId);

            _logger.LogInformation("?ã xóa user trong Identity Service: {AuthUserId}", authUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi xóa user trong Identity Service: {AuthUserId}", authUserId);
            throw;
        }
    }

    public virtual async Task SetUserActiveInIdentityAsync(
        Guid authUserId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("C?p nh?t tr?ng thái user trong Identity Service: {AuthUserId} - {IsActive}", authUserId, isActive);

            var user = await _identityUserAppService.GetAsync(authUserId);
            var updateDto = new IdentityUserUpdateDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                IsActive = isActive,
                ConcurrencyStamp = user.ConcurrencyStamp
            };

            await _identityUserAppService.UpdateAsync(authUserId, updateDto);

            _logger.LogInformation("?ã c?p nh?t tr?ng thái user trong Identity Service: {AuthUserId}", authUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi c?p nh?t tr?ng thái user trong Identity Service: {AuthUserId}", authUserId);
            throw;
        }
    }

    public virtual async Task<bool> IsUserExistInIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _identityUserAppService.GetAsync(authUserId);
            return user != null;
        }
        catch (EntityNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi ki?m tra user trong Identity Service: {AuthUserId}", authUserId);
            return false;
        }
    }

    public virtual async Task<(string UserName, string Email, string? FullName, string? PhoneNumber, bool IsActive)?> GetUserFromIdentityAsync(
        Guid authUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _identityUserAppService.GetAsync(authUserId);
            return (user.UserName, user.Email, user.Name, user.PhoneNumber, user.IsActive);
        }
        catch (EntityNotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi l?y thông tin user t? Identity Service: {AuthUserId}", authUserId);
            return null;
        }
    }
}