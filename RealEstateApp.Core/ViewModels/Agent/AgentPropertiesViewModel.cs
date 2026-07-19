using RealEstateApp.Core.ViewModels.Property;

namespace RealEstateApp.Core.ViewModels.Agent;

public class AgentPropertiesViewModel
{
    public string AgentId { get; set; } = string.Empty;
    public string NombreAgente { get; set; } = string.Empty;
    public string? FotoAgente { get; set; }
    public PropertyBrowseViewModel Propiedades { get; set; } = new();
}
