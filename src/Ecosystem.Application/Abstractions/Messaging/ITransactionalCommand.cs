﻿namespace Ecosystem.Application.Abstractions.Messaging;

public interface ITransactionalCommand : ICommand
{
}

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>
{
}
