using FluentValidation;

namespace Ecosystem.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Validator cho LoginCommand để đảm bảo dữ liệu đầu vào hợp lệ.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Tên đăng nhập là bắt buộc.")
            .MaximumLength(255)
            .WithMessage("Tên đăng nhập không vượt quá 255 ký tự.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu là bắt buộc.")
            .MinimumLength(1)
            .WithMessage("Mật khẩu không được để trống.")
            .MaximumLength(100)
            .WithMessage("Mật khẩu không vượt quá 100 ký tự.");
    }
} 