using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
}

public class UpdateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSupplierCommand>
{
    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Suppliers
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Update(request.Name, request.Phone);

        await context.SaveChangesAsync(cancellationToken);
    }
}
