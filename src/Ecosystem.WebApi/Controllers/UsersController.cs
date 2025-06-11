using Ecosystem.Application.Abstractions.Security;
using Ecosystem.Application.Features.Users.Queries.GetUser;
using Ecosystem.Domain.Aggregates.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecosystem.WebApi.Controllers;

public class UsersController : ApiControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator; // Inject để tạo token test

    public UsersController(ISender sender, IJwtTokenGenerator jwtTokenGenerator) : base(sender)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpGet("me")]
    [Authorize] // Endpoint này yêu cầu phải xác thực
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetUserByIdQuery(userId);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    // --- ENDPOINT ĐẶC BIỆT CHỈ DÙNG CHO MỤC ĐÍCH THỬ NGHIỆM ---
    [HttpGet("generate-test-token")]
    [AllowAnonymous] // Bất kỳ ai cũng có thể gọi
    public IActionResult GenerateTestToken()
    {
        // Tạo một user giả để sinh token
        // Trong thực tế, bạn sẽ lấy user từ DB
        var user = Domain.Aggregates.User.User.CreateFromIdp(
            Guid.NewGuid(),
            "testuser",
            Domain.ValueObjects.FullName.Create("Test", "User").Value,
            Domain.ValueObjects.Email.Create("test@example.com").Value
        ).Value;

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
