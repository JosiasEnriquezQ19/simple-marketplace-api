using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int DireccionId { get; set; }

        // Método de pago opcional (puede ser null)
        public int? MetodoPagoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoEnvio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Impuestos { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "pendiente";

        public DateTime FechaPedido { get; set; }

        [MaxLength(100)]
        public string? NumeroSeguimiento { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Direccion Direccion { get; set; } = null!;
        public MetodoPago? MetodoPago { get; set; } // Ahora opcional

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
        public Factura? Factura { get; set; }
    }
}
