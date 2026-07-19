using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Property;

// Forma compartida por el Home publico/cliente y por "propiedades de un
// agente": mismos filtros combinables, mismo listado. Busqueda por Codigo es
// una via alterna al filtrado normal (si viene Codigo, ignora los demas
// filtros y busca esa unica propiedad disponible).
public class PropertyBrowseViewModel : IValidatableObject
{
    [Display(Name = "Código de propiedad")]
    public string? Codigo { get; set; }

    [Display(Name = "Tipo de propiedad")]
    public int? PropertyTypeId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser menor que cero.")]
    [Display(Name = "Precio mínimo")]
    public decimal? PrecioMinimo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser menor que cero.")]
    [Display(Name = "Precio máximo")]
    public decimal? PrecioMaximo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
    [Display(Name = "Cantidad de habitaciones")]
    public int? CantidadHabitaciones { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
    [Display(Name = "Cantidad de baños")]
    public int? CantidadBanos { get; set; }

    public List<SelectOption> TiposPropiedad { get; set; } = new();
    public List<PropertyListItemViewModel> Propiedades { get; set; } = new();
    public string? Mensaje { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PrecioMinimo.HasValue && PrecioMaximo.HasValue && PrecioMinimo > PrecioMaximo)
            yield return new ValidationResult(
                "El precio mínimo no puede ser mayor que el precio máximo.",
                new[] { nameof(PrecioMinimo), nameof(PrecioMaximo) });
    }
}
