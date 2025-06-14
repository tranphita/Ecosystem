using Ecosystem.Application.Abstractions.Messaging;
using Ecosystem.Application.Features.Users.DTOs;

namespace Ecosystem.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Lệnh đăng nhập người dùng, xác thực với IDP bên ngoài và trả về JWT nội bộ.
/// </summary>
/// <param name="Username">Tên đăng nhập hoặc email</param>
/// <param name="Password">Mật khẩu người dùng</param>
public sealed record LoginCommand(
    string Username,
    string Password) : ITransactionalCommand<AuthenticationResult>
{
    public override string ToString() =>
        $"{nameof(LoginCommand)}: {{ Username = {Username}, Password = \"***\" }}";
} 