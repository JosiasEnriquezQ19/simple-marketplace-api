namespace SimpleMarketplace.Api.DTOs
{
    public class DireccionDto
    {
        public int DireccionId { get; set; }
        public int UsuarioId { get; set; }
    public string Calle { get; set; } = null!;
    public string Ciudad { get; set; } = null!;
    public string Estado { get; set; } = null!;
        public string CodigoPostal { get; set; } = null!;
        public string Pais { get; set; } = null!;
        public bool EsPrincipal { get; set; }
    // Note: 'Estado' above is the region/state for address; soft-delete flag isn't present in this table.
    }

    public class CrearDireccionDto
    {
        public int UsuarioId { get; set; }
        public string Calle { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
    public string? Estado { get; set; }
        public string CodigoPostal { get; set; } = null!;
        public string? Pais { get; set; }
        public bool EsPrincipal { get; set; }
    }

    public class UpdateDireccionDto
    {
        public string? Calle { get; set; }
        public string? Ciudad { get; set; }
    public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; }
        public bool? EsPrincipal { get; set; }
    }
}
