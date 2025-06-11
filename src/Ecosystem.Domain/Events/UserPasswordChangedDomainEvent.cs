using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.Primitives;

namespace Ecosystem.Domain.Events;

/// <summary>
/// Sự kiện miền cho biết mật khẩu của người dùng đã được thay đổi.
/// </summary>
public sealed record UserPasswordChangedDomainEvent(
    /// <summary>
    /// Định danh duy nhất của sự kiện.
    /// </summary>
    Guid Id,

    /// <summary>
    /// Định danh của người dùng có mật khẩu đã thay đổi.
    /// </summary>
    UserId UserId) : DomainEvent(Id);
