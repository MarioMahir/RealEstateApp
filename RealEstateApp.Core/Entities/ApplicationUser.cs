using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    // Requerida solo para Administrador/Desarrollador (alta hecha por un admin).
    // El auto-registro publico de Cliente/Agente no la pide.
    public string? Cedula { get; set; }

    public string? FotoUrl { get; set; }

    // Estado de negocio (Activo/Inactivo), decidido explicitamente por un
    // administrador o por el propio flujo de activacion. Deliberadamente
    // independiente de LockoutEnabled/LockoutEnd de Identity, que es un bloqueo
    // temporal automatico por intentos fallidos de login (concepto distinto).
    public bool Activo { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
