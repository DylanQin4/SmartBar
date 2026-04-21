using SmartBar.Domain.Features.Catalog.Enums;

namespace SmartBar.Application.Features.Catalog.Products.Queries.GetProducts;

public record ProductDto(
    Guid Id,
    string Name,
    ProductCategory Category,
    bool IsSellable,
    bool IsIngredient,
    decimal SellingPrice,
    decimal? NightPrice,
    UnitOfMeasure BaseUnit,
    bool IsActive);
