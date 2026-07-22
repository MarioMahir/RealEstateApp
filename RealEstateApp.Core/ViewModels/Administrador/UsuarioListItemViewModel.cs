namespace RealEstateApp.Core.ViewModels.Administrador;

public class UsuarioListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
