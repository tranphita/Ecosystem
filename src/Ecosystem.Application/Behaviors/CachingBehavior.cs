using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Ecosystem.Shared;

namespace Ecosystem.Application.Behaviors;

/// <summary>
/// Pipeline behavior để tự động cache các request có implement ICacheableQuery.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
    where TResponse : Result
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Thử lấy kết quả từ cache
        var cachedResponse = await _cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", request.CacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for {CacheKey}", request.CacheKey);

        // Nếu không có trong cache, thực thi handler
        var response = await next();

        // Chỉ cache khi thao tác thành công
        if (response.IsSuccess)
        {
            await _cacheService.SetAsync(request.CacheKey, response, request.Expiration, cancellationToken);
        }

        return response;
    }
} 