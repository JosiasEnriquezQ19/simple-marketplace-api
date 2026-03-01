namespace SimpleMarketplace.Api.DTOs
{
    public class CategoriaDto
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
        public string Estado { get; set; } = "activo";
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class CrearCategoriaDto
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class UpdateCategoriaDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Estado { get; set; }
    }
}
