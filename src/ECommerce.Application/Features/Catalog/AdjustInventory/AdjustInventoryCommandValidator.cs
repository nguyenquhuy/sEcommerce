using FluentValidation;

namespace ECommerce.Application.Features.Catalog.AdjustInventory;

public class AdjustInventoryCommandValidator : AbstractValidator<AdjustInventoryCommand>
{
    public AdjustInventoryCommandValidator()
    {
        RuleFor(x => x.VariantId).NotEmpty();
        RuleFor(x => x.Delta).NotEqual(0).WithMessage("Delta phải khác 0.");
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
