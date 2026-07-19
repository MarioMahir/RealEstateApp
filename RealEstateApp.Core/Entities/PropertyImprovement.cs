namespace RealEstateApp.Core.Entities;

// Tabla de union explicita N:M entre Property e Improvement. Se modela de forma
// explicita (en vez de dejar que EF Core la genere de forma implicita) porque
// el negocio tiene una regla propia sobre ella: al eliminar una Mejora, solo se
// deben borrar las filas de esta tabla, nunca las Property asociadas.
public class PropertyImprovement
{
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public int ImprovementId { get; set; }
    public Improvement Improvement { get; set; } = null!;
}
