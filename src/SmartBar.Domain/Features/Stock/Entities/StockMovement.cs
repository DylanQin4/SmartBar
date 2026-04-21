using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Stock.Enums;

namespace SmartBar.Domain.Features.Stock.Entities;

public class StockMovement : AggregateRoot
{
    public DateTime Date { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public StockDirection Direction { get; private set; }
    public StockMovementType MovementType { get; private set; }
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public Money? UnitValue { get; private set; }
    public string? Notes { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(
        Guid productId,
        decimal quantity,
        StockDirection direction,
        StockMovementType movementType,
        string? referenceType,
        Guid? referenceId,
        Money? unitValue,
        string? notes,
        Guid createdByUserId)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        return new StockMovement
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            ProductId = productId,
            Quantity = quantity,
            Direction = direction,
            MovementType = movementType,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            UnitValue = unitValue,
            Notes = notes,
            CreatedByUserId = createdByUserId
        };
    }
}
