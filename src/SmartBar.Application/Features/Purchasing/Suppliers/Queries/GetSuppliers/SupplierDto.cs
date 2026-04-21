namespace SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSuppliers;

public record SupplierDto(
    Guid Id,
    string Name,
    string? Phone,
    bool IsActive);
