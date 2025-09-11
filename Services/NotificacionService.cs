using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SimpleMarketplace.Api.Services
{
    public class NotificacionService
    {
        private readonly string _correoRemitente;
        private readonly string _nombreRemitente = "Tienda Ecommerce";
        private readonly string _contrasena;
        private readonly string _correoDestino;

        public NotificacionService()
        {
            _correoRemitente = Environment.GetEnvironmentVariable("EMAIL_REMITENTE") ?? "";
            _contrasena = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "";
            _correoDestino = Environment.GetEnvironmentVariable("EMAIL_DESTINO") ?? "";
        }

        public async Task EnviarCorreoPedidoAsync(string usuario, decimal monto)
        {
            var fromAddress = new MailAddress(_correoRemitente, _nombreRemitente);
            var toAddress = new MailAddress(_correoDestino);
            const string subject = "Nuevo pedido realizado";
            string body = $"El usuario {usuario} realizó un pedido por S/ {monto}.\n\nRevisa el panel de administración para más detalles.";

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _contrasena)
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
