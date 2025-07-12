using System;
using System.Threading;
using System.Threading.Tasks;
using Ecosystem.SmartBox.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Ecosystem.SmartBox.HealthChecks;

/// <summary>
/// Health Check cho k?t n?i ??n Identity Service
/// </summary>
public class IdentityServiceHealthCheck : IHealthCheck
{
    private readonly IUserBidirectionalSyncService _syncService;
    private readonly ILogger<IdentityServiceHealthCheck> _logger;

    public IdentityServiceHealthCheck(
        IUserBidirectionalSyncService syncService,
        ILogger<IdentityServiceHealthCheck> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Test connection by trying to check if a non-existent user exists
            var testUserId = Guid.NewGuid();
            var isConnected = await _syncService.IsUserExistInIdentityAsync(testUserId, cancellationToken);
            
            _logger.LogDebug("Identity Service health check completed successfully");
            
            return HealthCheckResult.Healthy("Identity Service connection is healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Identity Service health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Identity Service connection failed", 
                ex,
                new System.Collections.Generic.Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["timestamp"] = DateTime.UtcNow
                });
        }
    }
}