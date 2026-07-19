using RealEstateApp.Core.Enums;

namespace RealEstateApp.Core.Entities;

public class Offer
{
    public int Id { get; set; }

    public string ClienteId { get; set; } = string.Empty;
    public ApplicationUser Cliente { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public OfferStatus Estado { get; set; } = OfferStatus.Pendiente;
}
