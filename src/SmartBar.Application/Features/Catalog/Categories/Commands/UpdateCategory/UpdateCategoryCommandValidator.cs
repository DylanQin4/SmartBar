using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Catalog.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(BeUniqueName)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");

        RuleFor(v => v.Description)
            .MaximumLength(250);
    }

    private async Task<bool> BeUniqueName(UpdateCategoryCommand model, string name, CancellationToken cancellationToken)
    {
        return !await _context.Categories
            .Where(c => c.Id != model.Id)
            .AnyAsync(c => c.Name == name, cancellationToken);
    }
}
