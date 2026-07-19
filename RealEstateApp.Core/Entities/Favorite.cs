namespace RealEstateApp.Core.Entities;

// Clave compuesta (ClienteId, PropertyId), sin Id propio: un cliente no puede
// tener la misma propiedad marcada como favorita mas de una vez.
public class Favorite
{
    public string ClienteId { get; set; } = string.Empty;
    public ApplicationUser Cliente { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
