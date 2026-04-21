using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Catalog.Products.Commands.DeactivateProduct;

public record DeactivateProductCommand(Guid Id) : IRequest;

public class DeactivateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeactivateProductCommand>
{
    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
