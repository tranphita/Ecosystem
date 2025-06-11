using Ecosystem.Application.Features.Users.DTOs;

namespace Ecosystem.Application.Abstractions.Data;
public interface IUserReadRepository
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
