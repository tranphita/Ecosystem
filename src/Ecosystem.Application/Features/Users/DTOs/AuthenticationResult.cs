namespace Ecosystem.Application.Features.Users.DTOs;
public record AuthenticationResult(
    Guid UserId,
    string Token);
