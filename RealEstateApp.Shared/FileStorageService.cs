using Microsoft.Extensions.Options;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Shared;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

    private readonly FileStorageSettings _settings;

    public FileStorageService(IOptions<FileStorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> SaveImageAsync(Stream content, string originalFileName, string subfolder)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("El archivo seleccionado no tiene un formato de imagen válido.");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_settings.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(fileStream);

        return $"/uploads/{subfolder}/{fileName}";
    }
}
