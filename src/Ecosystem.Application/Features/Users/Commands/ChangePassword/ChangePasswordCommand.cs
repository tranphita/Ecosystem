using Ecosystem.Application.Abstractions.Messaging;

namespace Ecosystem.Application.Features.Users.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : ICommand;
