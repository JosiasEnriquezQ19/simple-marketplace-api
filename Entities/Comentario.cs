using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMarketplace.Api.Entities
{
    [Table("Comentarios")]
    public class Comentario
    {
        [Key]
        [Column("comentarioId")]
        public int ComentarioId { get; set; }

        [Required]
        [Column("productoId")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        [Required]
        [Column("usuarioId")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [Column("comentario")]
        public string Texto { get; set; } = null!;

        [Required]
        [Column("estrellas")]
        public int Estrellas { get; set; }

        [Column("fechaComentario")]
        public DateTime FechaComentario { get; set; }
    }
}
