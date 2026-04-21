using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Mappers;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrderById;

public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<PurchaseOrderDetailDto>;

public class GetPurchaseOrderByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDetailDto>
{
    public async Task<PurchaseOrderDetailDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.PurchaseOrders
            .AsNoTracking()
            .Include(o => o.Lines)
            .Include(o => o.Payments)
            .Where(o => o.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return entity.ToDetailDto();
    }
}
