using FluentValidation;

namespace ECommerce.Application.Features.Addresses.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^(0|\+84)\d{9,10}$").WithMessage("Số điện thoại không hợp lệ.");
        RuleFor(x => x.Province).NotEmpty().MaximumLength(100);
        RuleFor(x => x.District).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Ward).MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(300);
    }
}
