using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class ImprovementConfiguration : IEntityTypeConfiguration<Improvement>
{
    public void Configure(EntityTypeBuilder<Improvement> builder)
    {
        builder.Property(i => i.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Descripcion).IsRequired();
        builder.HasIndex(i => i.Nombre).IsUnique();
    }
}
