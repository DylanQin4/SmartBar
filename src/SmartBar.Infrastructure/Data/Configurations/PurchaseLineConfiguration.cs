using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class PurchaseLineConfiguration : IEntityTypeConfiguration<PurchaseLine>
{
    public void Configure(EntityTypeBuilder<PurchaseLine> builder)
    {
        builder.Property(p => p.Quantity).HasPrecision(18, 4);
        builder.Property(p => p.RemainingQuantity).HasPrecision(18, 4);

        builder.OwnsOne(p => p.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(10);
        });

        builder.OwnsOne(p => p.LineTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("LineTotal").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("LineTotalCurrency").HasMaxLength(10);
        });

        builder.Navigation(p => p.UnitPrice).IsRequired();
        builder.Navigation(p => p.LineTotal).IsRequired();
    }
}
