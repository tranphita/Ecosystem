namespace Ecosystem.Domain.Primitives;

/// <summary>
/// Sự kiện miền (Domain Event) là một bản ghi trừu tượng đại diện cho một sự kiện xảy ra trong hệ thống.
/// </summary>
public abstract record DomainEvent(Guid Id)
{
    /// <summary>
    /// Thời điểm sự kiện xảy ra (UTC).
    /// </summary>
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
