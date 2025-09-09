using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Direcciones")]
    public class Direccion
    {
        [Key]
        public int DireccionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Calle { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Ciudad { get; set; } = null!;

    [MaxLength(100)]
    [Column("estado")]
    // The DB column `estado` exists; use it as the address' region/state field named `Estado` in the model.
    public string Estado { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string CodigoPostal { get; set; } = null!;

        [MaxLength(100)]
        public string Pais { get; set; } = "Perú";

        public bool EsPrincipal { get; set; } = false;

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
