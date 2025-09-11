using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SimpleMarketplace.Api.Services
{
    public class NotificacionService
    {
        private readonly string _correoRemitente = "mitiendaplus276@gmail.com";
        private readonly string _nombreRemitente = "Tienda Ecommerce";
        private readonly string _contrasena = "clzg koxq cffs dndz"; // Contraseña de aplicación Gmail
        private readonly string _correoDestino = "mitiendaplus276@gmail.com"; // Cambia aquí por tu correo personal si lo deseas

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
