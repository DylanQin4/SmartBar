using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Mappers;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery : IRequest<IReadOnlyCollection<PurchaseOrderDto>>;

public class GetPurchaseOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPurchaseOrdersQuery, IReadOnlyCollection<PurchaseOrderDto>>
{
    public async Task<IReadOnlyCollection<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        return await context.PurchaseOrders
            .AsNoTracking()
            .OrderByDescending(o => o.PurchaseDate)
            .Select(o => o.ToDto())
            .ToListAsync(cancellationToken);
    }
}
