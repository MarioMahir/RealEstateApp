using AutoMapper;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

public class MantenimientoAdministradoresController : StaffMaintenanceControllerBase
{
    public MantenimientoAdministradoresController(
        IAdministradorService administradorService, IAccountService accountService, IMapper mapper)
        : base(administradorService, accountService, mapper)
    {
    }

    protected override string Rol => Roles.Administrador;
    protected override bool AplicaAutoproteccion => true;

    protected override string Titulo => "Mantenimiento de administradores";
    protected override string EtiquetaCrear => "Crear administrador";
    protected override string EntidadSingular => "administrador";
    protected override string EmptyListMessage => "No existen administradores registrados.";
    protected override string NotFoundMessage => "El administrador seleccionado no existe.";
    protected override string CreateSuccessMessage => "El administrador fue creado correctamente.";
    protected override string UpdateSuccessMessage => "El administrador fue actualizado correctamente.";
    protected override string ActivateSuccessMessage => "El administrador fue activado correctamente.";
    protected override string DeactivateSuccessMessage => "El administrador fue inactivado correctamente.";
    protected override string CannotModifySelfMessage => "No puede editar su propio usuario desde este mantenimiento.";
    protected override string CannotDeactivateSelfMessage => "No puede inactivar a su propio usuario.";
    protected override string LastActiveMessage => "Debe existir al menos un administrador activo en el sistema.";
}
