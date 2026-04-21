using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand : IRequest<Guid>
{
    public Guid SupplierId { get; init; }
    public DateTime PurchaseDate { get; init; }
}

public class CreatePurchaseOrderCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<CreatePurchaseOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await context.Suppliers
            .FindAsync([request.SupplierId], cancellationToken);

        Guard.Against.NotFound(request.SupplierId, supplier);

        var userId = Guid.TryParse(user.Id, out var parsedId) ? parsedId : Guid.Empty;

        var entity = PurchaseOrder.Create(request.SupplierId, request.PurchaseDate, userId);

        context.PurchaseOrders.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
