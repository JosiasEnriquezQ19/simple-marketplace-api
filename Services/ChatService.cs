using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;

namespace SimpleMarketplace.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ChatService(ApplicationDbContext db, IConfiguration config, HttpClient httpClient)
        {
            _db = db;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> GetAiResponseAsync(string userMessage)
        {
            var q = userMessage.ToLower().Trim();

            // 1. Responder a saludos comunes
            if (q == "hola" || q == "buenos dias" || q == "buenas tardes")
            {
                return "¡Hola! Soy miTiBOT, tu asistente personal. ¿En qué puedo ayudarte hoy? Puedo buscar productos, decirte qué marcas manejamos o ayudarte con tus dudas sobre envíos.";
            }

            // 2. Lógica de búsqueda en Base de Datos
            var palabrasClave = q.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                 .Where(p => p.Length > 3) // Evitar palabras cortas como "con", "de"
                                 .ToList();

            // Si no hay palabras clave largas, usamos el mensaje completo
            if (palabrasClave.Count == 0 && q.Length > 0) palabrasClave.Add(q);

            var queryResult = _db.Productos.Where(p => p.Estado == "disponible");

            // Búsqueda simple por palabras clave
            foreach (var palabra in palabrasClave)
            {
                queryResult = queryResult.Where(p => p.Nombre.ToLower().Contains(palabra) || 
                                                     (p.Descripcion != null && p.Descripcion.ToLower().Contains(palabra)) ||
                                                     (p.Marca != null && p.Marca.ToLower().Contains(palabra)));
            }

            var encontrados = await queryResult.Take(3).ToListAsync();

            if (encontrados.Any())
            {
                var response = "¡Claro! He encontrado estos productos en nuestra base de datos que podrían interesarte:\n\n";
                foreach (var p in encontrados)
                {
                    response += $"• {p.Nombre} - S/ {p.Precio:N2}\n";
                }
                response += "\n¿Te gustaría que te ayude a buscarlos en la tienda?";
                return response;
            }

            // 3. Fallback inteligente basado en categorías si no hay productos
            if (q.Contains("envio") || q.Contains("delivery"))
            {
                return "Hacemos envíos a todo Lima y provincias. El costo depende de tu ubicación y se calcula automáticamente al finalizar tu compra.";
            }

            if (q.Contains("pago") || q.Contains("yape") || q.Contains("plin"))
            {
                return "Aceptamos Yape, Plin y transferencias bancarias directas. ¡Es súper rápido y seguro!";
            }

            // 4. Default cuando no encuentra nada
            return $"No he encontrado productos específicos que coincidan con '{userMessage}', pero puedes intentar buscar por marca o categoría. ¿Hay algo más en lo que pueda ayudarte?";
        }
    }
}
