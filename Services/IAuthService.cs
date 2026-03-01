using SimpleMarketplace.Api.Entities;
using Google.Apis.Auth;

namespace SimpleMarketplace.Api.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken);
        string GenerateJwtToken(Usuario usuario);
        string GenerateAdminJwtToken(Administrador admin);
    }
}
