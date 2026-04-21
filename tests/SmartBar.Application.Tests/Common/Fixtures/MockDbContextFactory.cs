using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Application.Tests.Common.Fixtures;

/// <summary>
/// Creates a mock IApplicationDbContext backed by in-memory DbSets
/// so handler tests can Add/Find/Query without a real database.
/// </summary>
public static class MockDbContextFactory
{
    public static IApplicationDbContext Create()
    {
        var context = Substitute.For<IApplicationDbContext>();

        context.Categories.Returns(_ => CreateDbSet<Category>());
        context.Products.Returns(_ => CreateDbSet<Product>());
        context.Suppliers.Returns(_ => CreateDbSet<Supplier>());
        context.PurchaseOrders.Returns(_ => CreateDbSet<PurchaseOrder>());
        context.PurchaseLines.Returns(_ => CreateDbSet<PurchaseLine>());
        context.SupplierPayments.Returns(_ => CreateDbSet<SupplierPayment>());

        context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        return context;
    }

    private static DbSet<T> CreateDbSet<T>() where T : class
    {
        var data = new List<T>().AsQueryable();
        var dbSet = Substitute.For<DbSet<T>, IQueryable<T>>();

        ((IQueryable<T>)dbSet).Provider.Returns(data.Provider);
        ((IQueryable<T>)dbSet).Expression.Returns(data.Expression);
        ((IQueryable<T>)dbSet).ElementType.Returns(data.ElementType);
        ((IQueryable<T>)dbSet).GetEnumerator().Returns(data.GetEnumerator());

        return dbSet;
    }
}
