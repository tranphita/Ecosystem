namespace Ecosystem.Domain.Aggregates.Role;

public readonly record struct RoleId(Guid Value)
{
    public static RoleId New() => new(Guid.NewGuid());
}
