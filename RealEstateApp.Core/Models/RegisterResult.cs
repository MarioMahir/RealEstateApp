namespace RealEstateApp.Core.Models;

// Resultado propio (en vez de exponer IdentityResult directamente) para que
// Core no dependa de mas superficie de ASP.NET Identity que la estrictamente
// necesaria (IdentityUser/IdentityRole).
public class RegisterResult
{
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; } = new();
}
