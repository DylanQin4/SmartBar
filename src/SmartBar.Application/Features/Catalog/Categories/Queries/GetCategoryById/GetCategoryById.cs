using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Categories.Mappers;

namespace SmartBar.Application.Features.Catalog.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDetailDto>;

public class GetCategoryByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDetailDto>
{
    public async Task<CategoryDetailDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return entity.ToDetailDto();
    }
}
