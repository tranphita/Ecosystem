using Ecosystem.Application.Abstractions.Messaging;
using Ecosystem.Application.Features.Users.DTOs;

namespace Ecosystem.Application.Features.Users.Queries.GetUser;

/// <summary>
/// Query để lấy thông tin user theo ID.
/// </summary>
/// <param name="UserId">ID của user cần lấy.</param>
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>
{

} 