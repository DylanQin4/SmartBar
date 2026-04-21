using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Products.Mappers;

namespace SmartBar.Application.Features.Catalog.Products.Queries.GetProducts;

public record GetProductsQuery(bool ActiveOnly = true) : IRequest<IReadOnlyCollection<ProductDto>>;

public class GetProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking();

        if (request.ActiveOnly)
            query = query.Where(p => p.IsActive);

        return await query
            .OrderBy(p => p.Name)
            .Select(p => p.ToDto())
            .ToListAsync(cancellationToken);
    }
}
