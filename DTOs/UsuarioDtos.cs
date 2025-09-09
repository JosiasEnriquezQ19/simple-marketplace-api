namespace SimpleMarketplace.Api.DTOs
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Email { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Telefono { get; set; }
    public string Estado { get; set; } = "activo";
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    // Detailed DTO returned for single-user views in admin panel
    public class UsuarioDetailDto : UsuarioDto
    {
        public List<DireccionDto> Direcciones { get; set; } = new List<DireccionDto>();
        public List<MetodoPagoDto> MetodosPago { get; set; } = new List<MetodoPagoDto>();
        public List<PedidoDto> Pedidos { get; set; } = new List<PedidoDto>();
    }

    public class CrearUsuarioDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Telefono { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UpdateUsuarioDto
    {
    // opcional: permitir enviar el id del usuario en el body cuando el frontend llama PUT /api/Auth/me
    public int? UsuarioId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Password { get; set; }
        public string? Estado { get; set; }
    }
}
