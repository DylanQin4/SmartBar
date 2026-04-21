using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Products.Mappers;

namespace SmartBar.Application.Features.Catalog.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailDto>;

public class GetProductByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    public async Task<ProductDetailDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .AsNoTracking()
            .Include(p => p.UnitConversions)
            .Where(p => p.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return entity.ToDetailDto();
    }
}
