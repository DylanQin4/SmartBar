using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Categories.Mappers;

namespace SmartBar.Application.Features.Catalog.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<IReadOnlyCollection<CategoryDto>>;

public class GetCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyCollection<CategoryDto>>
{
    public async Task<IReadOnlyCollection<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => c.ToDto())
            .ToListAsync(cancellationToken);
    }
}
