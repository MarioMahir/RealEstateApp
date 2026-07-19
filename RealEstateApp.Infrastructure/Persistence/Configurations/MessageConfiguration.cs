using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.Texto).IsRequired();

        builder.HasOne(m => m.Property)
            .WithMany(p => p.Messages)
            .HasForeignKey(m => m.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message tiene DOS FKs hacia ApplicationUser (Cliente y Agente). Ambas
        // deben ser Restrict: si alguna fuera Cascade, SQL Server rechaza la
        // migracion (dos rutas de cascada distintas hacia la misma tabla padre
        // desde la misma tabla hija). Sin coleccion inversa en ApplicationUser
        // a proposito (WithMany() sin argumento) -- el borrado real fluye via
        // Property (ver PropertyConfiguration).
        builder.HasOne(m => m.Cliente)
            .WithMany()
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Agente)
            .WithMany()
            .HasForeignKey(m => m.AgenteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
