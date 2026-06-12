using FluentValidation;

namespace ECommerce.Application.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8) // BR-18
            .Matches("[A-Za-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ cái.")
            .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone)
            .Matches(@"^(0|\+84)\d{9,10}$").WithMessage("Số điện thoại không hợp lệ (định dạng VN).")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
