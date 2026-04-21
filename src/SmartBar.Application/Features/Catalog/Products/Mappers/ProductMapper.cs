using SmartBar.Application.Features.Catalog.Products.Queries.GetProductById;
using SmartBar.Application.Features.Catalog.Products.Queries.GetProducts;
using SmartBar.Domain.Features.Catalog.Entities;

namespace SmartBar.Application.Features.Catalog.Products.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Category,
            entity.IsSellable,
            entity.IsIngredient,
            entity.SellingPrice.Amount,
            entity.NightPrice?.Amount,
            entity.BaseUnit,
            entity.IsActive);

    public static ProductDetailDto ToDetailDto(this Product entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Category,
            entity.IsSellable,
            entity.IsIngredient,
            entity.SellingPrice.Amount,
            entity.NightPrice?.Amount,
            entity.VolumeInMl,
            entity.BaseUnit,
            entity.LowStockThreshold,
            entity.IsActive,
            entity.UnitConversions.Select(uc => new UnitConversionDto(
                uc.Id, uc.FromUnit, uc.ToUnit, uc.Factor)).ToList(),
            entity.Created,
            entity.CreatedBy,
            entity.LastModified,
            entity.LastModifiedBy);
}
