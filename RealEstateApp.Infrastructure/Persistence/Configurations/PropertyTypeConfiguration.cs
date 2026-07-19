using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
{
    public void Configure(EntityTypeBuilder<PropertyType> builder)
    {
        builder.Property(pt => pt.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(pt => pt.Descripcion).IsRequired();
        builder.HasIndex(pt => pt.Nombre).IsUnique();
    }
}
