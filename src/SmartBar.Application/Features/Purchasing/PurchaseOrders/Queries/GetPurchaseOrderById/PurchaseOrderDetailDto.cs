using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrderById;

public record PurchaseOrderDetailDto(
    Guid Id,
    Guid SupplierId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    PurchaseStatus Status,
    DateTime PurchaseDate,
    Guid CreatedByUserId,
    List<PurchaseLineDto> Lines,
    List<SupplierPaymentDto> Payments,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset LastModified,
    string? LastModifiedBy);

public record PurchaseLineDto(
    Guid Id,
    Guid ProductId,
    decimal Quantity,
    decimal RemainingQuantity,
    UnitOfMeasure Unit,
    decimal UnitPrice,
    decimal LineTotal);

public record SupplierPaymentDto(
    Guid Id,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTime PaidAt);
