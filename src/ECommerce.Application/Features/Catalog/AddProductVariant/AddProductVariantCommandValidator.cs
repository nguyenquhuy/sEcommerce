using FluentValidation;

namespace ECommerce.Application.Features.Catalog.AddProductVariant;

public class AddProductVariantCommandValidator : AbstractValidator<AddProductVariantCommand>
{
    public AddProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ComparePrice).GreaterThanOrEqualTo(0).When(x => x.ComparePrice.HasValue);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.InitialOnHand).GreaterThanOrEqualTo(0);
    }
}
