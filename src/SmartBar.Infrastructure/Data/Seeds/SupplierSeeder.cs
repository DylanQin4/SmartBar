using Microsoft.EntityFrameworkCore;
using SmartBar.Domain.Features.Purchasing.Constants;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Infrastructure.Data.Seeds;

public static class SupplierSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Suppliers.AnyAsync(s => s.Id == SupplierDefaults.WalkInId))
            return;

        var walkIn = Supplier.Create(SupplierDefaults.WalkInName, null);

        context.Entry(walkIn).Property(s => s.Id).CurrentValue = SupplierDefaults.WalkInId;

        context.Suppliers.Add(walkIn);
        await context.SaveChangesAsync();
    }
}
