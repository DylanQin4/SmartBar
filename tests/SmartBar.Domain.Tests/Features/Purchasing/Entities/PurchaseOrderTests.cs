using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Entities;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Domain.Tests.Features.Purchasing.Entities;

public class PurchaseOrderTests
{
    private static PurchaseOrder CreateOrder() =>
        PurchaseOrder.Create(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());

    // --- Create ---

    [Fact]
    public void Create_ReturnsOrderWithPendingStatus()
    {
        var order = CreateOrder();

        Assert.Equal(PurchaseStatus.Pending, order.Status);
        Assert.Equal(0m, order.TotalAmount.Amount);
        Assert.Equal(0m, order.PaidAmount.Amount);
        Assert.Equal(0m, order.RemainingAmount.Amount);
        Assert.Empty(order.Lines);
        Assert.Empty(order.Payments);
    }

    // --- AddLine ---

    [Fact]
    public void AddLine_RecalculatesTotal()
    {
        var order = CreateOrder();

        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(500));

        Assert.Equal(5000m, order.TotalAmount.Amount);
        Assert.Equal(5000m, order.RemainingAmount.Amount);
        Assert.Single(order.Lines);
    }

    [Fact]
    public void AddLine_MultipleLines_SumsCorrectly()
    {
        var order = CreateOrder();

        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(500));
        order.AddLine(Guid.NewGuid(), 5, UnitOfMeasure.Bottle, new Money(20000));

        Assert.Equal(105000m, order.TotalAmount.Amount);
        Assert.Equal(2, order.Lines.Count);
    }

    [Fact]
    public void AddLine_ZeroQuantity_Throws()
    {
        var order = CreateOrder();

        Assert.Throws<ArgumentException>(() =>
            order.AddLine(Guid.NewGuid(), 0, UnitOfMeasure.Piece, new Money(500)));
    }

    // --- AddPayment ---

    [Fact]
    public void AddPayment_PartialPayment_SetsPartiallyPaid()
    {
        var order = CreateOrder();
        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(1000));

        order.AddPayment(new Money(5000), PaymentMethod.Cash, DateTime.UtcNow);

        Assert.Equal(PurchaseStatus.PartiallyPaid, order.Status);
        Assert.Equal(5000m, order.PaidAmount.Amount);
        Assert.Equal(5000m, order.RemainingAmount.Amount);
    }

    [Fact]
    public void AddPayment_FullPayment_SetsPaid()
    {
        var order = CreateOrder();
        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(1000));

        order.AddPayment(new Money(10000), PaymentMethod.Cash, DateTime.UtcNow);

        Assert.Equal(PurchaseStatus.Paid, order.Status);
        Assert.Equal(0m, order.RemainingAmount.Amount);
    }

    [Fact]
    public void AddPayment_ExceedsRemaining_Throws()
    {
        var order = CreateOrder();
        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(1000));

        Assert.Throws<InvalidOperationException>(() =>
            order.AddPayment(new Money(20000), PaymentMethod.Cash, DateTime.UtcNow));
    }

    [Fact]
    public void AddPayment_MultiplePayments_TrackCumulatively()
    {
        var order = CreateOrder();
        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(1000));

        order.AddPayment(new Money(3000), PaymentMethod.Cash, DateTime.UtcNow);
        order.AddPayment(new Money(7000), PaymentMethod.MobileMoney, DateTime.UtcNow);

        Assert.Equal(PurchaseStatus.Paid, order.Status);
        Assert.Equal(10000m, order.PaidAmount.Amount);
        Assert.Equal(2, order.Payments.Count);
    }

    // --- Cancel ---

    [Fact]
    public void Cancel_PendingOrder_Succeeds()
    {
        var order = CreateOrder();

        order.Cancel();

        Assert.Equal(PurchaseStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_PartiallyPaidOrder_Throws()
    {
        var order = CreateOrder();
        order.AddLine(Guid.NewGuid(), 10, UnitOfMeasure.Piece, new Money(1000));
        order.AddPayment(new Money(5000), PaymentMethod.Cash, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }
}
