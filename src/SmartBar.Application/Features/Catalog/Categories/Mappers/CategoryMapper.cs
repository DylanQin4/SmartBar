using SmartBar.Application.Features.Catalog.Categories.Queries.GetCategories;
using SmartBar.Application.Features.Catalog.Categories.Queries.GetCategoryById;
using SmartBar.Domain.Entites;

namespace SmartBar.Application.Features.Catalog.Categories.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this Category entity)
        => new(entity.Id, entity.Name, entity.Description);

    public static CategoryDetailDto ToDetailDto(this Category entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.Created,
            entity.CreatedBy,
            entity.LastModified,
            entity.LastModifiedBy);
}
