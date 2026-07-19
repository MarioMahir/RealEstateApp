namespace RealEstateApp.Shared;

public class FileStorageSettings
{
    // Fijado desde el Program.cs de cada host (builder.Environment.WebRootPath)
    // -- asi Shared no necesita referenciar IWebHostEnvironment del framework web.
    public string WebRootPath { get; set; } = string.Empty;
}
