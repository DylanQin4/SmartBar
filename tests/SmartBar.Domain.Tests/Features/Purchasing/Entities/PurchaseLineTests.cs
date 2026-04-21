using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Domain.Tests.Features.Purchasing.Entities;

public class PurchaseLineTests
{
    private static PurchaseLine CreateLineViaOrder(decimal quantity = 10, decimal unitPrice = 1000)
    {
        var order = PurchaseOrder.Create(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());
        return order.AddLine(Guid.NewGuid(), quantity, UnitOfMeasure.Piece, new Money(unitPrice));
    }

    [Fact]
    public void Create_CalculatesLineTotal()
    {
        var line = CreateLineViaOrder(quantity: 5, unitPrice: 2000);

        Assert.Equal(10000m, line.LineTotal.Amount);
    }

    [Fact]
    public void Create_SetsRemainingQuantityEqualToQuantity()
    {
        var line = CreateLineViaOrder(quantity: 10);

        Assert.Equal(10m, line.RemainingQuantity);
        Assert.False(line.IsFullyConsumed);
        Assert.True(line.HasRemainingStock);
    }

    // --- FIFO ConsumeFifo ---

    [Fact]
    public void ConsumeFifo_PartialConsumption_ReturnsConsumedAmount()
    {
        var line = CreateLineViaOrder(quantity: 10);

        var consumed = line.ConsumeFifo(3);

        Assert.Equal(3m, consumed);
        Assert.Equal(7m, line.RemainingQuantity);
        Assert.False(line.IsFullyConsumed);
    }

    [Fact]
    public void ConsumeFifo_ExactConsumption_FullyConsumes()
    {
        var line = CreateLineViaOrder(quantity: 10);

        var consumed = line.ConsumeFifo(10);

        Assert.Equal(10m, consumed);
        Assert.Equal(0m, line.RemainingQuantity);
        Assert.True(line.IsFullyConsumed);
    }

    [Fact]
    public void ConsumeFifo_MoreThanAvailable_ConsumesOnlyRemaining()
    {
        var line = CreateLineViaOrder(quantity: 5);

        var consumed = line.ConsumeFifo(10);

        Assert.Equal(5m, consumed); // only 5 available
        Assert.True(line.IsFullyConsumed);
    }

    [Fact]
    public void ConsumeFifo_AlreadyEmpty_ReturnsZero()
    {
        var line = CreateLineViaOrder(quantity: 5);
        line.ConsumeFifo(5); // exhaust

        var consumed = line.ConsumeFifo(3);

        Assert.Equal(0m, consumed);
    }

    [Fact]
    public void ConsumeFifo_NegativeQuantity_Throws()
    {
        var line = CreateLineViaOrder(quantity: 10);

        Assert.Throws<ArgumentException>(() => line.ConsumeFifo(-5));
    }

    [Fact]
    public void ConsumeFifo_ZeroQuantity_Throws()
    {
        var line = CreateLineViaOrder(quantity: 10);

        Assert.Throws<ArgumentException>(() => line.ConsumeFifo(0));
    }

    [Fact]
    public void ConsumeFifo_MultipleConsumptions_TracksCorrectly()
    {
        var line = CreateLineViaOrder(quantity: 10);

        line.ConsumeFifo(3);
        line.ConsumeFifo(4);
        var remaining = line.ConsumeFifo(5);

        Assert.Equal(3m, remaining); // only 3 left after consuming 7
        Assert.True(line.IsFullyConsumed);
    }
}
