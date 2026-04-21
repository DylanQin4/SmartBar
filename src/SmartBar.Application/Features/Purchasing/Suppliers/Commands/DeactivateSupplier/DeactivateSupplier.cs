using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Commands.DeactivateSupplier;

public record DeactivateSupplierCommand(Guid Id) : IRequest;

public class DeactivateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeactivateSupplierCommand>
{
    public async Task Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Suppliers
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
