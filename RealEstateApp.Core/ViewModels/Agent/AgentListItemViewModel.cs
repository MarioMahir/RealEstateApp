namespace RealEstateApp.Core.ViewModels.Agent;

public class AgentListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
}
