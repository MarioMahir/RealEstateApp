using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class PropertyImprovementConfiguration : IEntityTypeConfiguration<PropertyImprovement>
{
    public void Configure(EntityTypeBuilder<PropertyImprovement> builder)
    {
        builder.HasKey(pi => new { pi.PropertyId, pi.ImprovementId });

        // Eliminar una Property se lleva sus vinculos con mejoras.
        builder.HasOne(pi => pi.Property)
            .WithMany(p => p.PropertyImprovements)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Eliminar una Improvement solo quita el vinculo N:M, nunca la Property.
        builder.HasOne(pi => pi.Improvement)
            .WithMany(i => i.PropertyImprovements)
            .HasForeignKey(pi => pi.ImprovementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
