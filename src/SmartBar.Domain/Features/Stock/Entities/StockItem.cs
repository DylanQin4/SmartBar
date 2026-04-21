using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Stock.Enums;

namespace SmartBar.Domain.Features.Stock.Entities;

public class StockItem : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private StockItem() { }

    public static StockItem Create(Guid productId)
    {
        return new StockItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            QuantityOnHand = 0,
            LastUpdated = DateTime.UtcNow
        };
    }

    public void ApplyMovement(decimal quantity, StockDirection direction)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        QuantityOnHand = direction switch
        {
            StockDirection.In => QuantityOnHand + quantity,
            StockDirection.Out => QuantityOnHand - quantity,
            StockDirection.Adjust => quantity,
            _ => throw new InvalidOperationException($"Unknown stock direction: {direction}")
        };

        LastUpdated = DateTime.UtcNow;
    }

    public bool IsLowStock(int threshold) => QuantityOnHand <= threshold;
}
