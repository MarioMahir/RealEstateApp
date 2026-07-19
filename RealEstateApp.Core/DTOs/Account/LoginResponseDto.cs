namespace RealEstateApp.Core.DTOs.Account;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime Expiracion { get; set; }
}
