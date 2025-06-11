namespace Ecosystem.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; }

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Id.Equals(entity.Id);

    public bool Equals(Entity<TId>? other) => Equals((object?)other);

    public static bool operator ==(Entity<TId> a, Entity<TId> b) => a.Equals(b);

    public static bool operator !=(Entity<TId> a, Entity<TId> b) => !(a == b);

    public override int GetHashCode() => Id.GetHashCode();
}
