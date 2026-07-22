using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(6);

        builder.HasIndex(p => p.Codigo).IsUnique();

        builder.Property(p => p.Descripcion).IsRequired();
        builder.Property(p => p.Precio).HasPrecision(18, 2);
        builder.Property(p => p.TamanoTerreno).HasPrecision(18, 2);

        builder.HasOne(p => p.PropertyType)
            .WithMany(pt => pt.Properties)
            .HasForeignKey(p => p.PropertyTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.SaleType)
            .WithMany(st => st.Properties)
            .HasForeignKey(p => p.SaleTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Agent)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.AgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
