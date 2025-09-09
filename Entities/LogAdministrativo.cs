using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("LogsAdministrativos")]
    public class LogAdministrativo
    {
        [Key]
        public int LogId { get; set; }

        public int? AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; } = null!;

        public string? Detalles { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(255)]
        public string? UserAgent { get; set; }

        public DateTime FechaRegistro { get; set; }

        // Navegación
        public Administrador? Administrador { get; set; }
    }
}
