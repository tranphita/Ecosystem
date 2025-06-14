using Ecosystem.Application.Abstractions.Identity;
using Ecosystem.Application.Features.Users.Commands.SyncUser;
using Ecosystem.Application.Features.Users.DTOs;
using Ecosystem.Shared;
using MediatR;

namespace Ecosystem.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Xử lý lệnh đăng nhập, điều phối luồng xác thực với IDP và đồng bộ user nội bộ.
/// </summary>
internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ISender _sender;

    public LoginCommandHandler(
        IAuthenticationService authenticationService,
        ISender sender)
    {
        _authenticationService = authenticationService;
        _sender = sender;
    }

    public async Task<Result<AuthenticationResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Bước 1: Xác thực với IDP bên ngoài
        var loginResult = await _authenticationService.LoginAsync(
            request.Username,
            request.Password,
            cancellationToken);

        if (loginResult.IsFailure)
        {
            return Result<AuthenticationResult>.Failure(loginResult.Error);
        }

        // Bước 2: Lấy thông tin người dùng từ IDP
        var userInfoResult = await _authenticationService.GetUserInfoAsync(
            loginResult.Value.AccessToken,
            cancellationToken);

        if (userInfoResult.IsFailure)
        {
            return Result<AuthenticationResult>.Failure(userInfoResult.Error);
        }

        var userInfo = userInfoResult.Value;

        // Bước 3: Đồng bộ user với DB nội bộ và sinh JWT
        var syncUserCommand = new SyncUserCommand(
            userInfo.AuthId,
            userInfo.Username,
            userInfo.Email,
            userInfo.FirstName,
            userInfo.LastName);

        var syncResult = await _sender.Send(syncUserCommand, cancellationToken);

        if (syncResult.IsFailure)
        {
            return Result<AuthenticationResult>.Failure(syncResult.Error);
        }

        // Bước 4: Trả về kết quả xác thực nội bộ
        return Result<AuthenticationResult>.Success(syncResult.Value);
    }
} 