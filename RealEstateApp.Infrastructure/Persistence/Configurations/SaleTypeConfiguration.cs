using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class SaleTypeConfiguration : IEntityTypeConfiguration<SaleType>
{
    public void Configure(EntityTypeBuilder<SaleType> builder)
    {
        builder.Property(st => st.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(st => st.Descripcion).IsRequired();
        builder.HasIndex(st => st.Nombre).IsUnique();
    }
}
