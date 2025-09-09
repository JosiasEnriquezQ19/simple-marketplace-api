using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("MetodosPago")]
    public class MetodoPago
    {
        [Key]
        public int MetodoPagoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoTarjeta { get; set; } = null!;

        [Required]
        [MaxLength(4)]
        public string UltimosCuatroDigitos { get; set; } = null!;

        [Required]
        public int MesExpiracion { get; set; }

        [Required]
    [Column("añoExpiracion")]
    public int AñoExpiracion { get; set; }

        public bool EsPrincipal { get; set; } = false;

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
