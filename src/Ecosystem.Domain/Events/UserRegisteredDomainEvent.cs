using Ecosystem.Domain.Aggregates.User;
using Ecosystem.Domain.Primitives;

namespace Ecosystem.Domain.Events;

/// <summary>
/// Sự kiện miền được phát sinh khi một người dùng mới đăng ký thành công.
/// </summary>
public sealed record UserRegisteredDomainEvent(

    Guid Id,

    /// <summary>
    /// Định danh của người dùng vừa đăng ký.
    /// </summary>
    UserId UserId,

    /// <summary>
    /// Định danh xác thực của người dùng vừa đăng ký.
    /// </summary>
    Guid AuthId,

    /// <summary>
    /// Địa chỉ email của người dùng vừa đăng ký.
    /// </summary>
    string Email) : DomainEvent(Id);
