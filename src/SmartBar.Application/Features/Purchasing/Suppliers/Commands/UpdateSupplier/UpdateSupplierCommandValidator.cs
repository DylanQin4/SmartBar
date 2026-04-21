using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierCommandValidator(IApplicationDbContext context)
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

    private async Task<bool> BeUniqueName(UpdateSupplierCommand model, string name, CancellationToken cancellationToken)
    {
        return !await _context.Suppliers
            .Where(s => s.Id != model.Id)
            .AnyAsync(s => s.Name == name, cancellationToken);
    }
}
