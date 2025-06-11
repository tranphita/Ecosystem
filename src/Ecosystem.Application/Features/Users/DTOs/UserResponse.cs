namespace Ecosystem.Application.Features.Users.DTOs;

/// <summary>
/// Response DTO cho User entity.
/// </summary>
public sealed record UserResponse(
    Guid Id,
    Guid AuthId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string? DisplayName,
    string? ProfilePictureUrl,
    string Status,
    DateTime CreatedOnUtc,
    DateTime? LastLoginOnUtc,
    bool IsEmailVerified); 