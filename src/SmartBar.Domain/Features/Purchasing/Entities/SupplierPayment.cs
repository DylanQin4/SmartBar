using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Domain.Features.Purchasing.Entities;

public class SupplierPayment : BaseEntity
{
    public Money Amount { get; private set; } = null!;
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime PaidAt { get; private set; }

    private SupplierPayment() { } // EF Core

    internal SupplierPayment(Money amount, PaymentMethod method, DateTime paidAt)
    {
        Guard.AgainstNegativeOrZero(amount.Amount, nameof(amount));

        Id = Guid.NewGuid();
        Amount = amount;
        PaymentMethod = method;
        PaidAt = paidAt;
    }
}
