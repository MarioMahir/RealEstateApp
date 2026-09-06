namespace RealEstateApp.Shared;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    /// <summary>Usuario SMTP. Vacío = modo desarrollo: los correos se guardan como archivos HTML.</summary>
    public string User { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    public string DisplayName { get; set; } = "RealEstateApp";

    /// <summary>Carpeta (relativa a la raíz del host) donde se escriben los correos simulados.</summary>
    public string OutputFolder { get; set; } = "App_Data/correos";

    public bool IsSmtpConfigured =>
        !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(User);
}
