using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Purchasing.Suppliers.Mappers;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDetailDto>;

public class GetSupplierByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSupplierByIdQuery, SupplierDetailDto>
{
    public async Task<SupplierDetailDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        return entity.ToDetailDto();
    }
}
