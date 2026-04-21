using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.ValueObjects;
using SmartBar.Domain.Features.Purchasing.Enums;

namespace SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddSupplierPayment;

public record AddSupplierPaymentCommand : IRequest<Guid>
{
    public Guid PurchaseOrderId { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public DateTime PaidAt { get; init; }
}

public class AddSupplierPaymentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AddSupplierPaymentCommand, Guid>
{
    public async Task<Guid> Handle(AddSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await context.PurchaseOrders
            .Include(o => o.Lines)
            .Include(o => o.Payments)
            .Where(o => o.Id == request.PurchaseOrderId)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.PurchaseOrderId, order);

        var payment = order.AddPayment(
            new Money(request.Amount),
            request.PaymentMethod,
            request.PaidAt);

        await context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
