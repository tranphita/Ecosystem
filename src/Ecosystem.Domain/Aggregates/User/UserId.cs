namespace Ecosystem.Domain.Aggregates.User;

public readonly record struct UserId(Guid Value)
{
    /// <summary>
    /// Tạo mới một UserId với giá trị ngẫu nhiên.
    /// </summary>
    public static UserId New() => new(Guid.NewGuid());
}