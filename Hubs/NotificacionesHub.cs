using Microsoft.AspNetCore.SignalR;

namespace SimpleMarketplace.Api.Hubs
{
    public class NotificacionesHub : Hub
    {
        // Este hub se utilizará para enviar notificaciones en tiempo real a los clientes
        // No necesitamos implementar métodos personalizados aquí ya que usaremos 
        // la funcionalidad básica de SignalR para enviar notificaciones

        public override async Task OnConnectedAsync()
        {
            // Se ejecuta cuando un cliente se conecta
            await Clients.Caller.SendAsync("connectionEstablished", "Conexión establecida con el hub de notificaciones");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Se ejecuta cuando un cliente se desconecta
            await base.OnDisconnectedAsync(exception);
        }
    }
}
