namespace RealEstateApp.Core.ViewModels.Administrador;

// "Listado de los agentes" -- muestra todos (activos e inactivos).
public class AgentAdminListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int CantidadPropiedades { get; set; }
    public bool Activo { get; set; }
}
