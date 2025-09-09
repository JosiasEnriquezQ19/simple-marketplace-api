namespace SimpleMarketplace.Api.DTOs
{
    public class ConfiguracionDto
    {
        public int ConfigId { get; set; }
        public string Clave { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public string? Descripcion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class CrearConfiguracionDto
    {
        public string Clave { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
