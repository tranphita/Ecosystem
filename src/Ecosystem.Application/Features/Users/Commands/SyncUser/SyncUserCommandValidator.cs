using FluentValidation;

namespace Ecosystem.Application.Features.Users.Commands.SyncUser;

/// <summary>
/// Validator cho SyncUserCommand để đảm bảo dữ liệu đầu vào hợp lệ.
/// </summary>
public sealed class SyncUserCommandValidator : AbstractValidator<SyncUserCommand>
{
    public SyncUserCommandValidator()
    {
        RuleFor(x => x.AuthId)
            .NotEmpty()
            .WithMessage("AuthId is required.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(100)
            .WithMessage("Username must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9._-]+$")
            .WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters.");
    }
} 