using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(f => new { f.ClienteId, f.PropertyId });

        builder.HasOne(f => f.Property)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict hacia ApplicationUser: todo el cascade real fluye a traves de
        // Property, nunca directo desde el usuario (evita "multiple cascade
        // paths" de SQL Server al converger Offer/Message/Favorite sobre Property
        // y sobre ApplicationUser a la vez).
        builder.HasOne(f => f.Cliente)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
