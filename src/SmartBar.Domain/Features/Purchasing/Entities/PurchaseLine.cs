using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Domain.Features.Purchasing.Entities;

public class PurchaseLine : BaseEntity
{
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public UnitOfMeasure Unit { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money LineTotal { get; private set; } = null!;

    private PurchaseLine() { } // EF Core

    internal PurchaseLine(Guid productId, decimal quantity, UnitOfMeasure unit, Money unitPrice)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.AgainstNegativeOrZero(unitPrice.Amount, nameof(unitPrice));

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        RemainingQuantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        LineTotal = unitPrice * quantity;
    }

    public decimal ConsumeFifo(decimal quantityNeeded)
    {
        if (RemainingQuantity == 0)
            return 0;

        var consumed = Math.Min(RemainingQuantity, quantityNeeded);
        RemainingQuantity -= consumed;
        return consumed;
    }

    public bool IsFullyConsumed => RemainingQuantity == 0;
    public bool HasRemainingStock => RemainingQuantity > 0;
}
