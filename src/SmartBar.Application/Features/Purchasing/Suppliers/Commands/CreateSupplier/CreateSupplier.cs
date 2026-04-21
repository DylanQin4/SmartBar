using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
}

public class CreateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSupplierCommand, Guid>
{
    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = Supplier.Create(request.Name, request.Phone);

        context.Suppliers.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
