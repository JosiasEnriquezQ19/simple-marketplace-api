namespace SimpleMarketplace.Api.DTOs
{
    public class BannerDto
    {
        public int BannerId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ImagenDesktopUrl { get; set; } = string.Empty;
        public string ImagenMobileUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearBannerDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string ImagenDesktopUrl { get; set; } = string.Empty;
        public string ImagenMobileUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; } = true;
    }
}
