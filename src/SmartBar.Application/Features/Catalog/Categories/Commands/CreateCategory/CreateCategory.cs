using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Entites;

namespace SmartBar.Application.Features.Catalog.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class CreateCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = Category.Create(request.Name, request.Description);

        context.Categories.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
