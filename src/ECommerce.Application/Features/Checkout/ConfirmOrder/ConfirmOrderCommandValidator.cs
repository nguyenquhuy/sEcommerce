using FluentValidation;

namespace ECommerce.Application.Features.Checkout.ConfirmOrder;

public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
{
    public ConfirmOrderCommandValidator()
    {
        RuleFor(x => x.ShippingMethod).NotEmpty();
        RuleFor(x => x.PaymentMethod).NotEmpty();
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.ShippingAddress).NotNull();
        RuleFor(x => x.ShippingAddress.RecipientName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShippingAddress.Phone)
            .NotEmpty().Matches(@"^(0|\+84)\d{9,10}$").WithMessage("Số điện thoại không hợp lệ.");
        RuleFor(x => x.ShippingAddress.Province).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShippingAddress.District).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShippingAddress.Street).NotEmpty().MaximumLength(300);
    }
}
