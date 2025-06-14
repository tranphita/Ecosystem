using Ecosystem.Application.Abstractions.Data;
using Ecosystem.Application.Common.Exceptions;
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
            throw new NotFoundException("User", request.UserId);
        }

        return user;
    }
}
