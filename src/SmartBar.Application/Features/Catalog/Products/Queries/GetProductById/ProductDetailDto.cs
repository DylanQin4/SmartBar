using SmartBar.Domain.Features.Catalog.Enums;

namespace SmartBar.Application.Features.Catalog.Products.Queries.GetProductById;

public record ProductDetailDto(
    Guid Id,
    string Name,
    ProductCategory Category,
    bool IsSellable,
    bool IsIngredient,
    decimal SellingPrice,
    decimal? NightPrice,
    int? VolumeInMl,
    UnitOfMeasure BaseUnit,
    int LowStockThreshold,
    bool IsActive,
    List<UnitConversionDto> UnitConversions,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset LastModified,
    string? LastModifiedBy);

public record UnitConversionDto(
    Guid Id,
    UnitOfMeasure FromUnit,
    UnitOfMeasure ToUnit,
    decimal Factor);
