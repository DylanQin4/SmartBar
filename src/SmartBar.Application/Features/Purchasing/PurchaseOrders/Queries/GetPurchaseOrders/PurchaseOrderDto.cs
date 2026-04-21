using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrders;

public record PurchaseOrderDto(
    Guid Id,
    Guid SupplierId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    PurchaseStatus Status,
    DateTime PurchaseDate);
