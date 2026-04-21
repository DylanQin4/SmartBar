using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddPurchaseLine;

public record AddPurchaseLineCommand : IRequest<Guid>
{
    public Guid PurchaseOrderId { get; init; }
    public Guid ProductId { get; init; }
    public decimal Quantity { get; init; }
    public UnitOfMeasure Unit { get; init; }
    public decimal UnitPrice { get; init; }
}

public class AddPurchaseLineCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AddPurchaseLineCommand, Guid>
{
    public async Task<Guid> Handle(AddPurchaseLineCommand request, CancellationToken cancellationToken)
    {
        var order = await context.PurchaseOrders
            .Include(o => o.Lines)
            .Include(o => o.Payments)
            .Where(o => o.Id == request.PurchaseOrderId)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.PurchaseOrderId, order);

        var product = await context.Products
            .FindAsync([request.ProductId], cancellationToken);

        Guard.Against.NotFound(request.ProductId, product);

        var line = order.AddLine(
            request.ProductId,
            request.Quantity,
            request.Unit,
            new Money(request.UnitPrice));

        await context.SaveChangesAsync(cancellationToken);

        return line.Id;
    }
}
