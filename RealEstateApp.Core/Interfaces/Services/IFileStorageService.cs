namespace RealEstateApp.Core.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(Stream content, string originalFileName, string subfolder);

    void DeleteImage(string relativeUrl);
}
