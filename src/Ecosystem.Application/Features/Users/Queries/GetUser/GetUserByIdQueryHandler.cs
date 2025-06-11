using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Features.Users.DTOs;
using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Features.Users.Queries.GetUser;

internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserReadRepository _userReadRepository;

    public GetUserByIdQueryHandler(IUserReadRepository userReadRepository)
    {
        _userReadRepository = userReadRepository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userReadRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("User.NotFound", "The user with the specified ID was not found."));
        }

        return user;
    }
}
