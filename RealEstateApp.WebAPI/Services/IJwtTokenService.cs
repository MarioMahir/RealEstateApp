using RealEstateApp.Core.Entities;

namespace RealEstateApp.WebAPI.Services;

public interface IJwtTokenService
{
    (string Token, DateTime Expiration) GenerateToken(ApplicationUser user, IList<string> roles);
}
