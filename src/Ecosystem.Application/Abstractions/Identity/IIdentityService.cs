using Ecosystem.Shared;

namespace Ecosystem.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<Result> ChangePasswordAsync(
        Guid authId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}