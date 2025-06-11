using Ecosystem.Application.Abstractions.Messaging;
using Ecosystem.Application.Features.Users.DTOs;
using Ecosystem.Shared;

namespace Ecosystem.Application.Features.Users.Commands.SyncUser;

/// <summary>
/// Lệnh đồng bộ thông tin người dùng với hệ thống xác thực.
/// </summary>
/// <param name="AuthId">Định danh xác thực của người dùng.</param>
/// <param name="Email">Địa chỉ email của người dùng.</param>
/// <param name="FirstName">Tên của người dùng.</param>
/// <param name="LastName">Họ của người dùng.</param>
/// <remarks>
/// Lệnh này được sử dụng để đồng bộ thông tin người dùng từ hệ thống xác thực bên ngoài vào hệ thống hiện tại.
/// </remarks>
public sealed record SyncUserCommand(
    Guid AuthId,
    string Username,
    string Email,
    string FirstName,
    string LastName) : ITransactionalCommand<AuthenticationResult>;
