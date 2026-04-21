using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Catalog.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(p => p.SellingPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SellingPrice").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("SellingPriceCurrency").HasMaxLength(10);
        });

        builder.OwnsOne(p => p.NightPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("NightPrice").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("NightPriceCurrency").HasMaxLength(10);
        });

        builder.Navigation(p => p.SellingPrice).IsRequired();

        builder.OwnsMany(p => p.UnitConversions, uc =>
        {
            uc.ToTable("ProductUnitConversions");
            uc.WithOwner().HasForeignKey("ProductId");
            uc.HasKey(nameof(Domain.Features.Catalog.ValueObjects.UnitConversion.Id));
            uc.Property(u => u.Factor).HasPrecision(18, 6);
        });
    }
}
