namespace SimpleMarketplace.Api.DTOs
{
    public class MetodoPagoDto
    {
        public int MetodoPagoId { get; set; }
        public int UsuarioId { get; set; }
        public string TipoTarjeta { get; set; } = null!;
        public string UltimosCuatroDigitos { get; set; } = null!;
        public int MesExpiracion { get; set; }
    public int AñoExpiracion { get; set; }
        public bool EsPrincipal { get; set; }
    // Nombre del titular (usuario)
    public string? Titular { get; set; }
    }

    public class CrearMetodoPagoDto
    {
        public int UsuarioId { get; set; }
        public string TipoTarjeta { get; set; } = null!;
        public string UltimosCuatroDigitos { get; set; } = null!;
        public int MesExpiracion { get; set; }
    public int AñoExpiracion { get; set; }
        public bool EsPrincipal { get; set; }
    }

    public class UpdateMetodoPagoDto
    {
        public string? TipoTarjeta { get; set; }
        public string? UltimosCuatroDigitos { get; set; }
        public int? MesExpiracion { get; set; }
    public int? AñoExpiracion { get; set; }
        public bool? EsPrincipal { get; set; }
    }
}
