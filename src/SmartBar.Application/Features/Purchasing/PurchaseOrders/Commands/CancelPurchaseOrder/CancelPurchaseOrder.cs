using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CancelPurchaseOrder;

public record CancelPurchaseOrderCommand(Guid Id) : IRequest;

public class CancelPurchaseOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CancelPurchaseOrderCommand>
{
    public async Task Handle(CancelPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.PurchaseOrders
            .Include(o => o.Payments)
            .Where(o => o.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, order);

        order.Cancel();

        await context.SaveChangesAsync(cancellationToken);
    }
}
