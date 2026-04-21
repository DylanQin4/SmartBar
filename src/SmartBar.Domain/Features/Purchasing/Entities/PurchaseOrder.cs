using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Domain.Features.Purchasing.Entities;

public class PurchaseOrder : AggregateRoot
{
    private readonly List<PurchaseLine> _lines = new();
    private readonly List<SupplierPayment> _payments = new();

    public Guid SupplierId { get; private set; }
    public Money TotalAmount { get; private set; } = Money.Zero();
    public Money PaidAmount { get; private set; } = Money.Zero();
    public Money RemainingAmount { get; private set; } = Money.Zero();
    public PurchaseStatus Status { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public IReadOnlyList<PurchaseLine> Lines => _lines.AsReadOnly();
    public IReadOnlyList<SupplierPayment> Payments => _payments.AsReadOnly();

    private PurchaseOrder() { }

    public static PurchaseOrder Create(Guid supplierId, DateTime purchaseDate, Guid createdByUserId)
    {
        return new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            PurchaseDate = purchaseDate,
            CreatedByUserId = createdByUserId,
            Status = PurchaseStatus.Pending,
            TotalAmount = Money.Zero(),
            PaidAmount = Money.Zero(),
            RemainingAmount = Money.Zero()
        };
    }

    public PurchaseLine AddLine(Guid productId, decimal quantity, UnitOfMeasure unit, Money unitPrice)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.AgainstNegativeOrZero(unitPrice.Amount, nameof(unitPrice));

        var line = new PurchaseLine(productId, quantity, unit, unitPrice);
        _lines.Add(line);
        RecalculateAmounts();
        return line;
    }

    public SupplierPayment AddPayment(Money amount, PaymentMethod method, DateTime paidAt)
    {
        Guard.AgainstNegativeOrZero(amount.Amount, nameof(amount));

        if (amount.Amount > RemainingAmount.Amount)
            throw new InvalidOperationException(
                $"Payment amount ({amount.Amount}) exceeds remaining amount ({RemainingAmount.Amount})");

        var payment = new SupplierPayment(amount, method, paidAt);
        _payments.Add(payment);
        RecalculateAmounts();
        return payment;
    }

    public void Cancel()
    {
        if (Status != PurchaseStatus.Pending)
            throw new InvalidOperationException("Only pending purchase orders can be cancelled");

        Status = PurchaseStatus.Cancelled;
    }

    private void RecalculateAmounts()
    {
        TotalAmount = _lines.Aggregate(Money.Zero(), (sum, line) => sum + line.LineTotal);
        PaidAmount = _payments.Aggregate(Money.Zero(), (sum, payment) => sum + payment.Amount);
        RemainingAmount = TotalAmount - PaidAmount;

        Status = PaidAmount.Amount switch
        {
            0 => PurchaseStatus.Pending,
            _ when PaidAmount.Amount < TotalAmount.Amount => PurchaseStatus.PartiallyPaid,
            _ => PurchaseStatus.Paid
        };
    }
}
