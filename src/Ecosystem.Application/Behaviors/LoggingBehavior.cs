using MediatR;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ecosystem.Application.Behaviors;

/// <summary>
/// Pipeline behavior để log tất cả các requests và performance metrics.
/// </summary>
/// <typeparam name="TRequest">Loại request.</typeparam>
/// <typeparam name="TResponse">Loại response.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Starting request {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation("Completed request {RequestName} in {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "Request {RequestName} failed after {ElapsedMilliseconds}ms: {ErrorMessage}", requestName, stopwatch.ElapsedMilliseconds, ex.Message);

            throw;
        }
    }
}