using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.Property(o => o.Monto).HasPrecision(18, 2);

        builder.HasOne(o => o.Property)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ver nota en FavoriteConfiguration sobre por que este lado es Restrict.
        builder.HasOne(o => o.Cliente)
            .WithMany(u => u.Offers)
            .HasForeignKey(o => o.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
