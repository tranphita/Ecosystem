using Ecosystem.Application.Features.Authentication.Commands.Login;
using Ecosystem.WebApi.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecosystem.WebApi.Controllers;

/// <summary>
/// Controller xử lý các chức năng xác thực (authentication).
/// </summary>
[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    public AuthController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Đăng nhập bằng tài khoản người dùng, trả về JWT nội bộ.
    /// </summary>
    /// <param name="request">Thông tin đăng nhập (username, password)</param>
    /// <param name="cancellationToken">Token huỷ thao tác</param>
    /// <returns>Kết quả xác thực và JWT</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);
        
        var result = await Sender.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.Error.Code switch
            {
                "Authentication.Failed" => Unauthorized(result.Error.Message),
                "User.NotFound" => NotFound(result.Error.Message),
                _ => BadRequest(result.Error.Message)
            };
    }
} 