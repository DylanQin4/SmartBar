using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Stock.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.Property(s => s.QuantityOnHand)
            .HasPrecision(18, 4);

        builder.HasIndex(s => s.ProductId)
            .IsUnique();
    }
}
