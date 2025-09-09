using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Facturas")]
    public class Factura
    {
        [Key]
        public int FacturaId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NumeroFactura { get; set; } = null!;

        public DateTime FechaEmision { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Impuestos { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(20)]
        public string EstadoPago { get; set; } = "pendiente";

        // Navegación
        public Pedido Pedido { get; set; } = null!;
    }
}
