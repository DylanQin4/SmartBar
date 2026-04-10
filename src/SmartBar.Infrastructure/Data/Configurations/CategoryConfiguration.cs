using Microsoft.EntityFrameworkCore;
using SmartBar.Domain.Features.Catalog.Entities;

namespace SmartBar.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.Description)
            .HasMaxLength(250);
    }
}
