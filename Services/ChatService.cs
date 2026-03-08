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
            var apiKey = _config["GeminiConfig:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "TU_API_KEY_AQUI")
            {
                return "Lo siento, el servicio de IA no está configurado correctamente. Por favor, asegúrate de añadir tu Gemini API Key.";
            }

            // 1. Obtener contexto de productos de la base de datos
            var productos = await _db.Productos
                .Where(p => p.Estado == "disponible")
                .Take(15) // Tomamos los primeros 15 para no saturar el prompt
                .Select(p => new { p.Nombre, p.Precio, p.Marca, p.Descripcion })
                .ToListAsync();

            var contextoProductos = string.Join("\n", productos.Select(p => 
                $"- {p.Nombre}: S/ {p.Precio:N2}. Marca: {p.Marca}. {p.Descripcion}"));

            // 2. Construir el prompt para Gemini
            var systemPrompt = $@"Eres 'miTiBOT', el asistente inteligente de la tienda 'MiTiendaPlus'. 
Tu objetivo es ayudar a los clientes de forma amable, profesional y eficiente. 

Contexto de la tienda (Productos disponibles):
{contextoProductos}

Reglas:
1. Si el usuario te pregunta por un producto que tenemos, recomiéndalo mencionando su precio.
2. Si el usuario pregunta por algo que NO tenemos, dile amablemente que no está en stock por ahora pero sugiérele algo similar si es posible.
3. Responde de forma concisa.
4. Si el usuario quiere comprar algo, dile que puede agregarlo al carrito.
5. Usa un tono peruano amable (puedes usar palabras como 'claro que sí', 'causa', 'chévere' de forma sutil y profesional si el cliente es informal).";

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = userMessage } }
                    }
                }
            };

            // 3. Llamar a la API de Gemini
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
            
            try
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonRequest = JsonSerializer.Serialize(requestBody, options);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Gemini-Error] {errorDetails}");
                    return "UPS! Tuve un pequeño problema cerebral al intentar responderte. ¿Podrías intentarlo de nuevo en un momento?";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                
                // Navegar por el JSON de respuesta de Gemini para obtener el texto
                var aiText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return aiText ?? "No pude generar una respuesta clara, pero estoy aquí para ayudarte.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat-Exception] {ex.Message}");
                return "Lo siento, mi conexión con la matriz ha fallado. Revisa tu internet o intenta más tarde.";
            }
        }
    }
}
