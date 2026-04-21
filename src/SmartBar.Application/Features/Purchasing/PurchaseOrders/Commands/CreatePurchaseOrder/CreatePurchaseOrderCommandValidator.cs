namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty();

        RuleFor(x => x.PurchaseDate)
            .NotEmpty();
    }
}
