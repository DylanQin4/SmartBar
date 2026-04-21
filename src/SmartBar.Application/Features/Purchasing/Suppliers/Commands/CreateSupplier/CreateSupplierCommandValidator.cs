using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueName)
            .WithMessage("A supplier with this name already exists.");

        RuleFor(x => x.Phone)
            .MaximumLength(50);
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Suppliers
            .AnyAsync(s => s.Name == name, cancellationToken);
    }
}
