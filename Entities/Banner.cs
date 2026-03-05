using System.ComponentModel.DataAnnotations;

namespace SimpleMarketplace.Api.Entities
{
    public class Banner
    {
        [Key]
        public int BannerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ImagenDesktopUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ImagenMobileUrl { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LinkUrl { get; set; }

        public int Orden { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
