using FluentValidation;

namespace ECommerce.Application.Features.Catalog.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(200)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ gồm chữ thường, số và dấu '-'.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
