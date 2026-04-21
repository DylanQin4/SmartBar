using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseLine> PurchaseLines { get; }
    DbSet<SupplierPayment> SupplierPayments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
