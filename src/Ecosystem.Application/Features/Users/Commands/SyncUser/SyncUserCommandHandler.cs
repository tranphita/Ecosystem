using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Abstractions.Security;
using Ecosystem.Application.Features.Users.DTOs;
using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.ValueObjects;
using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Features.Users.Commands.SyncUser;

internal sealed class SyncUserCommandHandler
    : IRequestHandler<SyncUserCommand, Result<AuthenticationResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private const string DefaultRoleName = "Member"; // Default role name

    public SyncUserCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(
        SyncUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        var fullNameResult = FullName.Create(request.FirstName, request.LastName);

        if (emailResult.IsFailure)
        {
            return Result<AuthenticationResult>.Failure(emailResult.Error);
        }

        if (fullNameResult.IsFailure)
        {
            return Result<AuthenticationResult>.Failure(fullNameResult.Error);
        }

        var user = await _unitOfWork.UserRepository.GetByAuthIdAsync(request.AuthId, cancellationToken);

        // Case 1: User does not exist -> Create new
        if (user is null)
        {
            var userResult = User.CreateFromIdp(
                request.AuthId,
                request.Username,
                fullNameResult.Value,
                emailResult.Value);

            if (userResult.IsFailure)
            {
                return Result<AuthenticationResult>.Failure(userResult.Error);
            }

            user = userResult.Value;

            var defaultRole = await _unitOfWork.RoleRepository.GetByNameAsync(DefaultRoleName, cancellationToken);
            if (defaultRole is null)
            {
                return Result<AuthenticationResult>.Failure(new Error("Role.NotFound", $"Default role '{DefaultRoleName}' not found."));
            }

            user.AddRole(defaultRole.Id);

            await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        }
        else // Case 2: User exists -> Sync information
        {
            user.SyncProfile(fullNameResult.Value, emailResult.Value);
        }

        user.TrackLogin();

        var localToken = _jwtTokenGenerator.GenerateToken(user);
        var authResult = new AuthenticationResult(user.Id.Value, localToken);

        return Result<AuthenticationResult>.Success(authResult);
    }
}
