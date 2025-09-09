using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Administradores")]
    public class Administrador
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("contraseñaHash")]
        public string ContrasenaHash { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }

    [MaxLength(20)]
    public string NivelAcceso { get; set; } = "basico";

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "activo";
    }
}
