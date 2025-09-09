using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Carrito")]
    public class CarritoItem
    {
        [Key]
        public int CarritoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;

        public DateTime FechaAgregado { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
