using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
