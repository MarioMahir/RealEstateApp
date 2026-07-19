using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.ViewModels.Cliente;

public class CreateOfferViewModel
{
    [Required]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Debe ingresar el monto de la oferta.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto de la oferta debe ser mayor que cero.")]
    [Display(Name = "Monto de la oferta (RD$)")]
    public decimal Monto { get; set; }
}
