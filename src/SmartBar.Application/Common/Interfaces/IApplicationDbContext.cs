using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Purchasing.Entities;
using SmartBar.Domain.Features.Stock.Entities;

namespace SmartBar.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseLine> PurchaseLines { get; }
    DbSet<SupplierPayment> SupplierPayments { get; }
    DbSet<StockItem> StockItems { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<OpenBottle> OpenBottles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
