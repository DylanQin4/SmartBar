namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddSupplierPayment;

public class AddSupplierPaymentCommandValidator : AbstractValidator<AddSupplierPaymentCommand>
{
    public AddSupplierPaymentCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();

        RuleFor(x => x.PaidAt)
            .NotEmpty();
    }
}
