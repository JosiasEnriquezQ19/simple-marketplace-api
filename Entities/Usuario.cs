using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

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

        [MaxLength(20)]
        public string? Telefono { get; set; }

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "activo";

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

        // Navegación
        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public ICollection<MetodoPago> MetodosPago { get; set; } = new List<MetodoPago>();
        public ICollection<CarritoItem> Carrito { get; set; } = new List<CarritoItem>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
