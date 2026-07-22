namespace RealEstateApp.Core.ViewModels.Administrador;

public class AgentAdminListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int CantidadPropiedades { get; set; }
    public bool Activo { get; set; }
}
