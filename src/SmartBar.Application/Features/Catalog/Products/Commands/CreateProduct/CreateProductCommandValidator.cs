using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Enums;

namespace SmartBar.Application.Features.Catalog.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueName)
            .WithMessage("A product with this name already exists.");

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.BaseUnit)
            .IsInEnum();

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0)
            .When(x => x.IsSellable)
            .WithMessage("Selling price must be greater than 0 for sellable products.");

        RuleFor(x => x.VolumeInMl)
            .NotNull()
            .GreaterThan(0)
            .When(x => x.Category == ProductCategory.Alcohol)
            .WithMessage("Volume is required for alcohol products.");

        RuleFor(x => x.NightPrice)
            .GreaterThan(0)
            .When(x => x.NightPrice.HasValue)
            .WithMessage("Night price must be greater than 0.");

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x)
            .Must(x => x.IsSellable || x.IsIngredient)
            .WithMessage("A product must be sellable or an ingredient (or both).");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Products
            .AnyAsync(p => p.Name == name, cancellationToken);
    }
}
