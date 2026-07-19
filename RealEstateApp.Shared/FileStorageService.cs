using Microsoft.Extensions.Options;
using RealEstateApp.Core.Interfaces.Services;

namespace RealEstateApp.Shared;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

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

        if (!content.CanSeek || !await MatchesImageSignatureAsync(content, extension))
            throw new InvalidOperationException("El archivo seleccionado no tiene un formato de imagen válido.");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_settings.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(fileStream);

        return $"/uploads/{subfolder}/{fileName}";
    }

    public void DeleteImage(string relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)) return;

        var relativePath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_settings.WebRootPath, relativePath);

        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException)
        {
            // Best-effort: un archivo bloqueado no debe impedir que la
            // operacion de base de datos (la que realmente importa) continue.
        }
    }

    // Valida los primeros bytes del archivo contra la firma real del formato
    // (no solo la extensión) para rechazar archivos renombrados/corruptos/vacíos.
    private static async Task<bool> MatchesImageSignatureAsync(Stream content, string extension)
    {
        var firmaEsperada = extension == ".png" ? PngSignature : JpegSignature;
        var encabezado = new byte[firmaEsperada.Length];
        var leidos = await content.ReadAtLeastAsync(encabezado, encabezado.Length, throwOnEndOfStream: false);
        content.Position = 0;

        return leidos == firmaEsperada.Length && encabezado.SequenceEqual(firmaEsperada);
    }
}
