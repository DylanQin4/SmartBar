using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Stock.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.Property(s => s.Quantity)
            .HasPrecision(18, 4);

        builder.Property(s => s.ReferenceType)
            .HasMaxLength(100);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.OwnsOne(s => s.UnitValue, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitValue").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("UnitValueCurrency").HasMaxLength(10);
        });

        builder.HasIndex(s => new { s.ProductId, s.Date });
    }
}
