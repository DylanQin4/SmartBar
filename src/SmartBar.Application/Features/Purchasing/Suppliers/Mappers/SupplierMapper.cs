using SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSupplierById;
using SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSuppliers;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Features.Purchasing.Suppliers.Mappers;

public static class SupplierMapper
{
    public static SupplierDto ToDto(this Supplier entity)
        => new(entity.Id, entity.Name, entity.Phone, entity.IsActive);

    public static SupplierDetailDto ToDetailDto(this Supplier entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Phone,
            entity.IsActive,
            entity.Created,
            entity.CreatedBy,
            entity.LastModified,
            entity.LastModifiedBy);
}
