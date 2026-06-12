using FluentValidation;

namespace ECommerce.Application.Features.Coupons.CreateCoupon;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Type).Must(t => t is "Percent" or "Fixed")
            .WithMessage("Type phải là 'Percent' hoặc 'Fixed'.");
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.Value).LessThanOrEqualTo(100).When(x => x.Type == "Percent")
            .WithMessage("Percent không vượt quá 100.");
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxDiscountAmount).GreaterThan(0).When(x => x.MaxDiscountAmount.HasValue);
        RuleFor(x => x.MaxUsage).GreaterThan(0).When(x => x.MaxUsage.HasValue);
        RuleFor(x => x.MaxUsagePerUser).GreaterThan(0);
        RuleFor(x => x.EndAt).GreaterThan(x => x.StartAt).WithMessage("EndAt phải sau StartAt.");
    }
}
