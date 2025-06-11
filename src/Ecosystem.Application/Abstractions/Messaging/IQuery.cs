using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}