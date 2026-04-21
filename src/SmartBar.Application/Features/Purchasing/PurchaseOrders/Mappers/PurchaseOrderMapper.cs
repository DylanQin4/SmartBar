using SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrderById;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrders;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Mappers;

public static class PurchaseOrderMapper
{
    public static PurchaseOrderDto ToDto(this PurchaseOrder entity)
        => new(
            entity.Id,
            entity.SupplierId,
            entity.TotalAmount.Amount,
            entity.PaidAmount.Amount,
            entity.RemainingAmount.Amount,
            entity.Status,
            entity.PurchaseDate);

    public static PurchaseOrderDetailDto ToDetailDto(this PurchaseOrder entity)
        => new(
            entity.Id,
            entity.SupplierId,
            entity.TotalAmount.Amount,
            entity.PaidAmount.Amount,
            entity.RemainingAmount.Amount,
            entity.Status,
            entity.PurchaseDate,
            entity.CreatedByUserId,
            entity.Lines.Select(l => new PurchaseLineDto(
                l.Id, l.ProductId, l.Quantity, l.RemainingQuantity,
                l.Unit, l.UnitPrice.Amount, l.LineTotal.Amount)).ToList(),
            entity.Payments.Select(p => new SupplierPaymentDto(
                p.Id, p.Amount.Amount, p.PaymentMethod, p.PaidAt)).ToList(),
            entity.Created,
            entity.CreatedBy,
            entity.LastModified,
            entity.LastModifiedBy);
}
