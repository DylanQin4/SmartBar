using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Purchasing.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.OwnsOne(p => p.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(10);
        });

        builder.OwnsOne(p => p.PaidAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("PaidAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("PaidAmountCurrency").HasMaxLength(10);
        });

        builder.OwnsOne(p => p.RemainingAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("RemainingAmount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("RemainingAmountCurrency").HasMaxLength(10);
        });

        builder.Navigation(p => p.TotalAmount).IsRequired();
        builder.Navigation(p => p.PaidAmount).IsRequired();
        builder.Navigation(p => p.RemainingAmount).IsRequired();

        builder.HasMany(p => p.Lines)
            .WithOne()
            .HasForeignKey("PurchaseOrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Payments)
            .WithOne()
            .HasForeignKey("PurchaseOrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
