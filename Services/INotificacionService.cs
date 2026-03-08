using System.Threading.Tasks;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Services
{
    public interface INotificacionService
    {
        Task EnviarMensajeTelegramAsync(string mensaje);
    }
}
