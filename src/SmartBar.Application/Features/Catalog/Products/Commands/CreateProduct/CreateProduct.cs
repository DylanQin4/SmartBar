using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Application.Features.Catalog.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Guid>
{
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

public class CreateProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var sellingPrice = new Money(request.SellingPrice);
        var nightPrice = request.NightPrice.HasValue ? new Money(request.NightPrice.Value) : null;

        var entity = Product.Create(
            request.Name,
            request.Category,
            request.IsSellable,
            request.IsIngredient,
            sellingPrice,
            nightPrice,
            request.VolumeInMl,
            request.BaseUnit,
            request.LowStockThreshold);

        context.Products.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
