using System.ComponentModel.DataAnnotations;
using RealEstateApp.Core.ViewModels;

namespace RealEstateApp.Core.ViewModels.Agente;

public class PropertyFormViewModel : IValidatableObject
{
    public int Id { get; set; }
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de propiedad.")]
    [Display(Name = "Tipo de propiedad")]
    public int? PropertyTypeId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de venta.")]
    [Display(Name = "Tipo de venta")]
    public int? SaleTypeId { get; set; }

    [Required(ErrorMessage = "Debe ingresar el precio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
    [Display(Name = "Precio (RD$)")]
    public decimal? Precio { get; set; }

    [Required(ErrorMessage = "Debe ingresar el tamaño del terreno.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño del terreno debe ser mayor que cero.")]
    [Display(Name = "Tamaño del terreno (m²)")]
    public decimal? TamanoTerreno { get; set; }

    [Required(ErrorMessage = "Debe ingresar la cantidad de habitaciones.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero.")]
    [Display(Name = "Cantidad de habitaciones")]
    public int? CantidadHabitaciones { get; set; }

    [Required(ErrorMessage = "Debe ingresar la cantidad de baños.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero.")]
    [Display(Name = "Cantidad de baños")]
    public int? CantidadBanos { get; set; }

    [Required(ErrorMessage = "Debe ingresar una descripción.")]
    [StringLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    public List<int> ImprovementIds { get; set; } = new();
    public List<int> ImagenesAEliminar { get; set; } = new();

    public List<SelectOption> TiposPropiedad { get; set; } = new();
    public List<SelectOption> TiposVenta { get; set; } = new();
    public List<SelectOption> TodasLasMejoras { get; set; } = new();
    public List<ExistingImageViewModel> ImagenesActuales { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ImprovementIds.Count == 0)
            yield return new ValidationResult(
                "Debe seleccionarse al menos una mejora.", new[] { nameof(ImprovementIds) });
    }
}
