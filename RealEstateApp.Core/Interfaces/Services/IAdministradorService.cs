using RealEstateApp.Core.ViewModels.Administrador;

namespace RealEstateApp.Core.Interfaces.Services;

public interface IAdministradorService
{
    Task<AdministradorHomeViewModel> GetIndicadoresAsync();
}
