namespace RealEstateApp.Core.DTOs.Agent;

public class AgentDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public int CantidadPropiedades { get; set; }
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Estado { get; set; }
}
