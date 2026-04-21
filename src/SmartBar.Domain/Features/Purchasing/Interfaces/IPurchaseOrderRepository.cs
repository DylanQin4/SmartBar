using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Domain.Features.Purchasing.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<List<PurchaseLine>> GetAvailableFifoLotsAsync(Guid productId, CancellationToken ct = default);
}
