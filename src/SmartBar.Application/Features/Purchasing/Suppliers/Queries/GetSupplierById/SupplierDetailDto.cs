namespace SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSupplierById;

public record SupplierDetailDto(
    Guid Id,
    string Name,
    string? Phone,
    bool IsActive,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset LastModified,
    string? LastModifiedBy);
