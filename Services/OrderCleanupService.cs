using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.Hubs;

namespace SimpleMarketplace.Api.Services
{
    public class OrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCleanupService> _logger;
        // Revisa cada hora
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
        // Cancela los pedidos pendientes que tengan más de 48 horas
        private readonly int _hoursToKeepPending = 48;

        public OrderCleanupService(IServiceProvider serviceProvider, ILogger<OrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Order Cleanup Service iniciado. Limpiará pedidos con más de {_hoursToKeepPending} horas en espera de pago.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingOrdersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error al procesar la limpieza de pedidos.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingOrdersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificacionesHub>>();

            // Calculamos la fecha límite (ahora - 48 horas)
            var limitDate = DateTime.UtcNow.AddHours(-_hoursToKeepPending);

            // Buscamos pedidos pendientes más antiguos que la fecha límite
            var pedidosAExpirar = await dbContext.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(p => p.Estado == "pendiente" && p.FechaPedido <= limitDate)
                .ToListAsync(stoppingToken);

            if (pedidosAExpirar.Any())
            {
                _logger.LogInformation($"Se encontraron {pedidosAExpirar.Count} pedidos pendientes caducados. Procediendo a cancelarlos y devolver el stock...");

                foreach (var pedido in pedidosAExpirar)
                {
                    pedido.Estado = "cancelado";
                    
                    // Al cancelar, devolvemos el stock reservado de vuelta a los productos
                    if (pedido.Detalles != null)
                    {
                        foreach (var detalle in pedido.Detalles)
                        {
                            if (detalle.Producto != null)
                            {
                                detalle.Producto.Stock += detalle.Cantidad;
                            }
                        }
                    }

                    try
                    {
                        // Notificar a todos (FrontEnd e Dashboard de Admin) que el pedido ha sido cancelado
                        await hubContext.Clients.All.SendAsync("PedidoActualizado", new { 
                            pedidoId = pedido.PedidoId, 
                            nuevoEstado = "cancelado" 
                        }, cancellationToken: stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Fallo al enviar notificación SignalR para el pedido {pedido.PedidoId}");
                    }
                }

                // Guardamos todos los cambios (estado y stock) a la vez
                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Se han cancelado {pedidosAExpirar.Count} pedidos exitosamente.");
            }
        }
    }
}
