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

        // Indice unico real ademas de la verificacion en PropertyService: cierra
        // la condicion de carrera del generador aleatorio de 6 digitos.
        builder.HasIndex(p => p.Codigo).IsUnique();

        builder.Property(p => p.Descripcion).IsRequired();
        builder.Property(p => p.Precio).HasPrecision(18, 2);
        builder.Property(p => p.TamanoTerreno).HasPrecision(18, 2);

        // Cascade solo hacia los catalogos y hacia si misma via Property; nunca
        // cascade directo desde ApplicationUser (ver comentario en PropertyImage
        // /Offer/Message/Favorite: el borrado de un Agente se resuelve en codigo).
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
