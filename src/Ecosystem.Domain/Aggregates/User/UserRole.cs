using Ecosystem.Domain.Aggregates.Role;
using Ecosystem.Domain.Primitives;

namespace Ecosystem.Domain.Aggregates.User;

public sealed class UserRole
{
    private UserRole(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

    public static UserRole Create(UserId userId, RoleId roleId)
    {
        return new UserRole(userId, roleId);
    }
}