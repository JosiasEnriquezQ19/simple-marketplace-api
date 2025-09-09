namespace SimpleMarketplace.Api.DTOs
{
    public class AdministradorDto
    {
        public int AdminId { get; set; }
        public string Email { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NivelAcceso { get; set; } = "basico";
    public string Estado { get; set; } = "activo";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
    }

    public class CrearAdministradorDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NivelAcceso { get; set; } = "basico";
    }
}
