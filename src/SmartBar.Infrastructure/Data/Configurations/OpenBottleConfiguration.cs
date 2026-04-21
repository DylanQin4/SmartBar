using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBar.Domain.Features.Stock.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class OpenBottleConfiguration : IEntityTypeConfiguration<OpenBottle>
{
    public void Configure(EntityTypeBuilder<OpenBottle> builder)
    {
        builder.Property(o => o.RemainingVolumeMl)
            .HasPrecision(18, 2);
    }
}
