namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddPurchaseLine;

public class AddPurchaseLineCommandValidator : AbstractValidator<AddPurchaseLineCommand>
{
    public AddPurchaseLineCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Unit)
            .IsInEnum();

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);
    }
}
