using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Entities;
using SmartBar.Domain.Features.Stock.Entities;
using SmartBar.Domain.Features.Stock.Enums;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddPurchaseLine;

public record AddPurchaseLineCommand : IRequest<Guid>
{
    public Guid PurchaseOrderId { get; init; }
    public Guid ProductId { get; init; }
    public decimal Quantity { get; init; }
    public UnitOfMeasure Unit { get; init; }
    public decimal UnitPrice { get; init; }
}

public class AddPurchaseLineCommandHandler(IApplicationDbContext context, IUser user)
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

        var unitPrice = new Money(request.UnitPrice);
        var line = order.AddLine(request.ProductId, request.Quantity, request.Unit, unitPrice);

        // Stock: convert to base unit and create movement
        var baseQty = product.ConvertToBaseUnit(request.Quantity, request.Unit);
        var userId = Guid.TryParse(user.Id, out var parsedId) ? parsedId : Guid.Empty;

        var movement = StockMovement.Create(
            request.ProductId,
            baseQty,
            StockDirection.In,
            StockMovementType.Purchase,
            nameof(PurchaseOrder),
            order.Id,
            unitPrice,
            null,
            userId);

        context.StockMovements.Add(movement);

        // Update or create StockItem
        var stockItem = await context.StockItems
            .SingleOrDefaultAsync(s => s.ProductId == request.ProductId, cancellationToken);

        if (stockItem is null)
        {
            stockItem = StockItem.Create(request.ProductId);
            context.StockItems.Add(stockItem);
        }

        stockItem.ApplyMovement(baseQty, StockDirection.In);

        await context.SaveChangesAsync(cancellationToken);

        return line.Id;
    }
}
