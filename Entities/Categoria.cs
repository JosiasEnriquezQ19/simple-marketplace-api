using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        [MaxLength(255)]
        public string? ImagenUrl { get; set; }

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "activo";

        [Column("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechaActualizacion")]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
