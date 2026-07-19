namespace RealEstateApp.Core.Interfaces.Services;

// Contrato en Core; implementacion (System.IO puro, sin tipos de ASP.NET Core)
// vive en RealEstateApp.Shared. Recibe Stream + nombre de archivo en vez de
// IFormFile a proposito: IFormFile es del framework web y Shared no debe
// depender de el.
public interface IFileStorageService
{
    Task<string> SaveImageAsync(Stream content, string originalFileName, string subfolder);
}
