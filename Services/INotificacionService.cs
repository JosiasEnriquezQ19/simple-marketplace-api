using System.Threading.Tasks;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Services
{
    public interface INotificacionService
    {
        Task EnviarCorreoAdminNuevoPedidoAsync(Pedido pedido, Usuario cliente);
        Task EnviarCorreoClienteNuevoPedidoAsync(Pedido pedido, Usuario cliente);
        Task EnviarMensajeTelegramAsync(string mensaje);
    }
}
