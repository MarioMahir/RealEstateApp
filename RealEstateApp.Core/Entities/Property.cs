using RealEstateApp.Core.Enums;

namespace RealEstateApp.Core.Entities;

public class Property
{
    public int Id { get; set; }

    // Codigo unico de 6 digitos, autogenerado, inmutable. Ver PropertyService
    // para la generacion; el indice unico real vive en la configuracion de EF.
    public string Codigo { get; set; } = string.Empty;

    public int PropertyTypeId { get; set; }
    public PropertyType PropertyType { get; set; } = null!;

    public int SaleTypeId { get; set; }
    public SaleType SaleType { get; set; } = null!;

    public string AgentId { get; set; } = string.Empty;
    public ApplicationUser Agent { get; set; } = null!;

    public decimal Precio { get; set; }
    public decimal TamanoTerreno { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    public PropertyStatus Estado { get; set; } = PropertyStatus.Disponible;
    public DateTime FechaCreacion { get; set; }

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
