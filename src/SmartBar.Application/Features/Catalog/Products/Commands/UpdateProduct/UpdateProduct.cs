using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Application.Features.Catalog.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
    public bool IsSellable { get; init; }
    public bool IsIngredient { get; init; }
    public decimal SellingPrice { get; init; }
    public decimal? NightPrice { get; init; }
    public int? VolumeInMl { get; init; }
    public UnitOfMeasure BaseUnit { get; init; }
    public int LowStockThreshold { get; init; }
}

public class UpdateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Products
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var sellingPrice = new Money(request.SellingPrice);
        var nightPrice = request.NightPrice.HasValue ? new Money(request.NightPrice.Value) : null;

        entity.Update(
            request.Name,
            request.Category,
            request.IsSellable,
            request.IsIngredient,
            sellingPrice,
            nightPrice,
            request.VolumeInMl,
            request.BaseUnit,
            request.LowStockThreshold);

        await context.SaveChangesAsync(cancellationToken);
    }
}
