using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Catalog.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;

public class DeleteCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .Where(c => c.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        context.Categories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}
