using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
