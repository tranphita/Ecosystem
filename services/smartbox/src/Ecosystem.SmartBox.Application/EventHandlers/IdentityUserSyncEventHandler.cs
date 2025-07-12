using System;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Services;
using Ecosystem.SmartBox.Users;
using Microsoft.Extensions.Logging;
using Polly;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Ecosystem.SmartBox.EventHandlers;

/// <summary>
/// Event Handler ?? x? lý s? ki?n t? Identity Service v?i retry mechanism
/// </summary>
public class IdentityUserSyncEventHandler : IDistributedEventHandler<IdentityUserSyncedIntegrationEvent>, ITransientDependency
{
    private readonly IUserBidirectionalSyncService _bidirectionalSyncService;
    private readonly ILogger<IdentityUserSyncEventHandler> _logger;

    public IdentityUserSyncEventHandler(
        IUserBidirectionalSyncService bidirectionalSyncService,
        ILogger<IdentityUserSyncEventHandler> logger)
    {
        _bidirectionalSyncService = bidirectionalSyncService;
        _logger = logger;
    }

    public virtual async Task HandleEventAsync(IdentityUserSyncedIntegrationEvent eventData)
    {
        // Create retry policy with exponential backoff
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount} for syncing user {AuthUserId} after {Delay}ms. Exception: {Exception}",
                        retryCount, eventData.AuthUserId, timespan.TotalMilliseconds, outcome.Message);
                });

        try
        {
            _logger.LogInformation("Nh?n event ??ng b? t? Identity Service: {AuthUserId} - {OperationType}", 
                eventData.AuthUserId, eventData.OperationType);

            await retryPolicy.ExecuteAsync(async () =>
            {
                // ??ng b? user t? Identity Service v? SmartBox
                var smartBoxUser = await _bidirectionalSyncService.SyncIdentityUserToSmartBoxAsync(
                    eventData.AuthUserId,
                    eventData.UserName,
                    eventData.Email,
                    eventData.FullName,
                    eventData.PhoneNumber,
                    eventData.IsActive
                );

                _logger.LogInformation("?ã ??ng b? user t? Identity Service: {AuthUserId} -> {SmartBoxUserId}", 
                    eventData.AuthUserId, smartBoxUser.Id);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i cu?i cùng khi x? lý event ??ng b? t? Identity Service: {AuthUserId}", eventData.AuthUserId);
            
            // Store failed sync operation for later retry
            StoreFailedSyncEventAsync(eventData, ex);
            
            throw; // Re-throw to ensure the event is marked as failed
        }
    }

    private void StoreFailedSyncEventAsync(IdentityUserSyncedIntegrationEvent eventData, Exception exception)
    {
        // TODO: Implement storage of failed sync events for later retry
        // This could involve saving to database, dead letter queue, etc.
        _logger.LogWarning("Storing failed sync event for user {AuthUserId} for later retry", eventData.AuthUserId);
    }
}