using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Purchasing.Suppliers.Mappers;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery(bool ActiveOnly = true) : IRequest<IReadOnlyCollection<SupplierDto>>;

public class GetSuppliersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSuppliersQuery, IReadOnlyCollection<SupplierDto>>
{
    public async Task<IReadOnlyCollection<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Suppliers.AsNoTracking();

        if (request.ActiveOnly)
            query = query.Where(s => s.IsActive);

        return await query
            .OrderBy(s => s.Name)
            .Select(s => s.ToDto())
            .ToListAsync(cancellationToken);
    }
}
