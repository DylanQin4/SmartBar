namespace SmartBar.Application.Features.Catalog.Categories.Queries.GetCategoryById;

public record CategoryDetailDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset LastModified,
    string? LastModifiedBy);
