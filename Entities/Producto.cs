using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; } = 0;

        [Required]
        [MaxLength(255)]
        public string ImagenUrl { get; set; } = null!;
    [MaxLength(255)]
    public string? ImagenUrl2 { get; set; }
    [MaxLength(255)]
    public string? ImagenUrl3 { get; set; }
    [MaxLength(255)]
    public string? ImagenUrl4 { get; set; }
    [MaxLength(255)]
    public string? ImagenUrl5 { get; set; }
    [MaxLength(255)]
    public string? ImagenUrl6 { get; set; }
    [MaxLength(255)]
    public string? ImagenUrl7 { get; set; }

        [Required]
        [MaxLength(50)]
        public string Categoria { get; set; } = null!;

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "disponible";

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    public ICollection<CarritoItem> Carritos { get; set; } = new List<CarritoItem>();
    // Removida la colección de ProductoImagen - ahora usamos ImagenUrl2..ImagenUrl7 en lugar de tabla separada
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}
