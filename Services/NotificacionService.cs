using System.Net;
using System.Net.Mail;
using SimpleMarketplace.Api.Entities;
using Microsoft.Extensions.Configuration;

namespace SimpleMarketplace.Api.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IConfiguration _config;
        private readonly string _server;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _password;
        private readonly string _adminEmail;

        public NotificacionService(IConfiguration config)
        {
            _config = config;
            _server = _config["EmailConfig:SmtpServer"] ?? "smtp.gmail.com";
            _port = int.Parse(_config["EmailConfig:Port"] ?? "587");
            _senderEmail = _config["EmailConfig:SenderEmail"] ?? "";
            _senderName = _config["EmailConfig:SenderName"] ?? "MiTiendaPlus";
            _password = _config["EmailConfig:AppPassword"] ?? "";
            _adminEmail = _config["EmailConfig:AdminEmail"] ?? "";
        }

        public async Task EnviarCorreoAdminNuevoPedidoAsync(Pedido pedido, Usuario cliente)
        {
            const string subject = "🚀 ¡NUEVO PEDIDO RECIBIDO! - MiTiendaPlus";
            string body = GetAdminTemplate(pedido, cliente);
            await EnviarEmailAsync(_adminEmail, subject, body);
        }

        public async Task EnviarCorreoClienteNuevoPedidoAsync(Pedido pedido, Usuario cliente)
        {
            if (string.IsNullOrEmpty(cliente.Email)) return;
            
            string subject = $"¡Gracias por su compra, {cliente.Nombre}! 🛍️";
            string body = GetCustomerTemplate(pedido, cliente);
            await EnviarEmailAsync(cliente.Email, subject, body);
        }

        private async Task EnviarEmailAsync(string dest, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress(_senderEmail, _senderName);
                var toAddress = new MailAddress(dest);

                using (var smtp = new SmtpClient
                {
                    Host = _server,
                    Port = _port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_senderEmail, _password)
                })
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Solo loggear el error para no detener el flujo del pedido
                Console.WriteLine($"Error enviando correo a {dest}: {ex.Message}");
            }
        }

        private string GetAdminTemplate(Pedido pedido, Usuario cliente)
        {
            return $@"
            <div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;"">
                <div style=""background: #1e293b; padding: 30px; text-align: center; color: white;"">
                    <h1 style=""margin: 0; font-size: 24px; letter-spacing: 1px;"">🚀 ¡NUEVO PEDIDO RECIBIDO!</h1>
                </div>
                <div style=""padding: 30px; background: #ffffff;"">
                    <div style=""background: #f8fafc; border-radius: 8px; padding: 20px; border-left: 4px solid #3b82f6; margin-bottom: 25px;"">
                        <p style=""margin: 0; font-weight: bold; color: #1e293b;"">ID del Pedido: #{pedido.PedidoId}</p>
                        <p style=""margin: 5px 0 0; color: #64748b;"">Total: S/ {pedido.Total:N2}</p>
                    </div>
                    <h3 style=""color: #1e293b; border-bottom: 1px solid #f1f5f9; padding-bottom: 10px;"">👤 Información del Cliente</h3>
                    <p style=""margin: 10px 0; color: #475569;""><strong>Nombre:</strong> {cliente.Nombre} {cliente.Apellido}</p>
                    <p style=""margin: 10px 0; color: #475569;""><strong>Email:</strong> {cliente.Email}</p>
                    <p style=""margin: 10px 0; color: #475569;""><strong>Teléfono:</strong> {cliente.Telefono ?? "-"}</p>
                    <div style=""margin-top: 40px; padding: 25px; background: #fff7ed; border-radius: 12px; border: 1px solid #ffedd5; text-align: center;"">
                        <h4 style=""margin: 0 0 10px; color: #9a3412;"">⚠️ Acción Requerida</h4>
                        <p style=""margin: 0; color: #c2410c; font-size: 14px;"">Verifica tu cuenta de <strong>Yape / Plin / Banco</strong>. Si el pago es correcto, cambia el estado del pedido a <strong>""Procesando""</strong>.</p>
                    </div>
                </div>
            </div>";
        }

        private string GetCustomerTemplate(Pedido pedido, Usuario cliente)
        {
            return $@"
            <div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 600px; margin: 0 auto; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);"">
                <div style=""background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); padding: 40px; text-align: center; color: white;"">
                    <h1 style=""margin: 0; font-size: 28px; font-weight: 800;"">¡Gracias por tu compra!</h1>
                </div>
                <div style=""padding: 40px; background: white;"">
                    <p style=""font-size: 16px; color: #4b5563;"">Hola <strong>{cliente.Nombre}</strong>,</p>
                    <p style=""font-size: 16px; color: #4b5563;"">Hemos recibido tu pedido con el ID <strong>#{pedido.PedidoId}</strong> por un total de <strong>S/ {pedido.Total:N2}</strong>. Estamos validando el pago para proceder con el envío.</p>
                    <div style=""background: #f3f4f6; border-radius: 12px; padding: 25px; margin: 30px 0;"">
                        <h4 style=""margin: 0 0 10px; color: #1f2937;"">Próximos pasos:</h4>
                        <p style=""margin: 5px 0; color: #4b5563;"">1. Validaremos tu pago en los próximos minutos.</p>
                        <p style=""margin: 5px 0; color: #4b5563;"">2. Recibirás una notificación cuando tu pedido esté en camino.</p>
                    </div>
                </div>
            </div>";
        }
    }
}
