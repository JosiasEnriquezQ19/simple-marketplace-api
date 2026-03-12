using System;
using System.Threading.Tasks;
using SimpleMarketplace.Api.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace SimpleMarketplace.Api.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IConfiguration _config;
        private readonly string _botToken;
        private readonly string _chatId;

        public NotificacionService(IConfiguration config)
        {
            _config = config;
            // Telegram Config - Standarized for Render/Environment variables
            _botToken = _config["Telegram:BotToken"] ?? _config["Telegram_BotToken"] ?? "";
            _chatId = _config["Telegram:ChatId"] ?? _config["Telegram_ChatId"] ?? "";
        }

        public async Task EnviarMensajeTelegramAsync(string mensaje)
        {
            if (string.IsNullOrEmpty(_botToken) || string.IsNullOrEmpty(_chatId)) return;

            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
                    var data = new
                    {
                        chat_id = _chatId,
                        text = mensaje,
                        parse_mode = "HTML"
                    };

                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(data),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("[Telegram-Success] Mensaje enviado correctamente.");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Telegram-Error] Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Telegram-Exception] {ex.Message}");
            }
        }
    }
}
