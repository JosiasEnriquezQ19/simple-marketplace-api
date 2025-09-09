namespace SimpleMarketplace.Api.DTOs
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = null!;
    public string? ImagenUrl2 { get; set; }
    public string? ImagenUrl3 { get; set; }
    public string? ImagenUrl4 { get; set; }
    public string? ImagenUrl5 { get; set; }
    public string? ImagenUrl6 { get; set; }
    public string? ImagenUrl7 { get; set; }
        // Array que contiene todas las URLs de imágenes (incluyendo la principal)
        public List<string> Imagenes { get; set; } = new List<string>();
        public string Categoria { get; set; } = null!;
    public string Estado { get; set; } = "disponible";
    }

    public class CrearProductoDto
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = null!;
    public string? ImagenUrl2 { get; set; }
    public string? ImagenUrl3 { get; set; }
    public string? ImagenUrl4 { get; set; }
    public string? ImagenUrl5 { get; set; }
    public string? ImagenUrl6 { get; set; }
    public string? ImagenUrl7 { get; set; }
        public string Categoria { get; set; } = null!;
    }

    public class UpdateProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public int? Stock { get; set; }
        public string? ImagenUrl { get; set; }
    public string? ImagenUrl2 { get; set; }
    public string? ImagenUrl3 { get; set; }
    public string? ImagenUrl4 { get; set; }
    public string? ImagenUrl5 { get; set; }
    public string? ImagenUrl6 { get; set; }
    public string? ImagenUrl7 { get; set; }
        public string? Categoria { get; set; }
        public string? Estado { get; set; }
    }
}
