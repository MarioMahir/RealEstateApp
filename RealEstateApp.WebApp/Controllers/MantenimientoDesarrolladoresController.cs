using AutoMapper;
using RealEstateApp.Core.Constants;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers;

// Sin autoproteccion a proposito: un Administrador nunca gestiona su propio
// registro aqui (su propio usuario es de rol Administrador, no Desarrollador
// -- son roles distintos y mutuamente excluyentes en este sistema).
public class MantenimientoDesarrolladoresController : StaffMaintenanceControllerBase
{
    public MantenimientoDesarrolladoresController(
        IAdministradorService administradorService, IAccountService accountService, IMapper mapper)
        : base(administradorService, accountService, mapper)
    {
    }

    protected override string Rol => Roles.Desarrollador;
    protected override bool AplicaAutoproteccion => false;

    protected override string Titulo => "Mantenimiento de desarrolladores";
    protected override string EtiquetaCrear => "Crear desarrollador";
    protected override string EntidadSingular => "desarrollador";
    protected override string EmptyListMessage => "No existen desarrolladores registrados.";
    protected override string NotFoundMessage => "El desarrollador seleccionado no existe.";
    protected override string CreateSuccessMessage => "El desarrollador fue creado correctamente.";
    protected override string UpdateSuccessMessage => "El desarrollador fue actualizado correctamente.";
    protected override string ActivateSuccessMessage => "El desarrollador fue activado correctamente.";
    protected override string DeactivateSuccessMessage => "El desarrollador fue inactivado correctamente.";
}
