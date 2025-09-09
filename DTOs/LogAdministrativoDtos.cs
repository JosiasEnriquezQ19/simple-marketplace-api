namespace SimpleMarketplace.Api.DTOs
{
    public class LogAdministrativoDto
    {
        public int LogId { get; set; }
        public int? AdminId { get; set; }
        public string Accion { get; set; } = null!;
        public string? Detalles { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CrearLogAdministrativoDto
    {
        public int? AdminId { get; set; }
        public string Accion { get; set; } = null!;
        public string? Detalles { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
