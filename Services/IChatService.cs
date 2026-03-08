using System.Threading.Tasks;

namespace SimpleMarketplace.Api.Services
{
    public interface IChatService
    {
        Task<string> GetAiResponseAsync(string userMessage);
    }
}
