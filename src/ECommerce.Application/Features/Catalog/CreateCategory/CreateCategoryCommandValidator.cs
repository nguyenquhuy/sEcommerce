using FluentValidation;

namespace ECommerce.Application.Features.Catalog.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(200)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ gồm chữ thường, số và dấu '-'.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
