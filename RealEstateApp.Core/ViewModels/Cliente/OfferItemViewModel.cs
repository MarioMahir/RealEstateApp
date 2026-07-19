namespace RealEstateApp.Core.ViewModels.Cliente;

public class OfferItemViewModel
{
    public int Id { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
}
