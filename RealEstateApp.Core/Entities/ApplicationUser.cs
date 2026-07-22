using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    public string? Cedula { get; set; }

    public string? FotoUrl { get; set; }

    public bool Activo { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
