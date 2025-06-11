using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Abstractions.Messaging;
using Ecosystem.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecosystem.Application.Behaviors;

/// <summary>
/// Pipeline behavior để wrap tất cả các commands trong transaction.
/// </summary>
/// <typeparam name="TRequest">Loại request.</typeparam>
/// <typeparam name="TResponse">Loại response.</typeparam>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionalCommand  // Chỉ áp dụng cho các request là Command
    where TResponse : Result
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Bắt đầu một transaction trong CSDL
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Starting database transaction for {RequestName}", requestName);

            // Chạy handler
            var response = await next();

            // Nếu handler chạy thành công (không ném exception), commit transaction
            await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

            _logger.LogInformation("Committed database transaction for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rolling back database transaction for {RequestName}", requestName);

            // Nếu có bất kỳ lỗi nào xảy ra, rollback transaction
            await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);

            throw; // Ném lại exception để các tầng trên xử lý
        }
    }
}