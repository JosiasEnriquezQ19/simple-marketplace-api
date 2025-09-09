using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Configuraciones")]
    public class Configuracion
    {
        [Key]
        public int ConfigId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Clave { get; set; } = null!;

        [Required]
        public string Valor { get; set; } = null!;

        [MaxLength(255)]
        public string? Descripcion { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
