using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Features.Catalog.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class UpdateCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Update(request.Name, request.Description);

        await context.SaveChangesAsync(cancellationToken);
    }
}
