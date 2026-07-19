using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Entities;

namespace RealEstateApp.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
    public DbSet<SaleType> SaleTypes => Set<SaleType>();
    public DbSet<Improvement> Improvements => Set<Improvement>();
    public DbSet<PropertyImprovement> PropertyImprovements => Set<PropertyImprovement>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Fluent API exclusivamente aqui: las entidades de Core quedan como POCOs
        // limpios, sin un solo atributo de EF Core.
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
