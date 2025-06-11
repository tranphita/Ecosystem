namespace Ecosystem.Domain.Primitives;

/// <summary>
/// Đại diện cho lớp cơ sở của các aggregate root trong mô hình miền.
/// Một aggregate root là một thực thể đóng vai trò là gốc của một aggregate và chịu trách nhiệm duy trì tính nhất quán của các thay đổi trong aggregate.
/// </summary>
/// <typeparam name="TId">Kiểu của định danh duy nhất cho aggregate root.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="AggregateRoot{TId}"/>.
    /// </summary>
    /// <param name="id">Định danh duy nhất của aggregate root.</param>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Lấy các sự kiện miền đã được phát sinh bởi aggregate root này.
    /// </summary>
    /// <returns>Một tập hợp chỉ đọc các sự kiện miền.</returns>
    public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.ToList();

    /// <summary>
    /// Xóa tất cả các sự kiện miền khỏi aggregate root.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Phát sinh một sự kiện miền và thêm nó vào danh sách các sự kiện miền.
    /// </summary>
    /// <param name="domainEvent">Sự kiện miền cần phát sinh.</param>
    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}