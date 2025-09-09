using System;

namespace SimpleMarketplace.Api.DTOs
{
    public class ComentarioDto
    {
        public int ComentarioId { get; set; }
        public int ProductoId { get; set; }
        public int UsuarioId { get; set; }
        public string Texto { get; set; } = null!;
        public int Estrellas { get; set; }
        public DateTime FechaComentario { get; set; }
    }

    public class CrearComentarioDto
    {
        public string Texto { get; set; } = null!;
        public int Estrellas { get; set; }
    }

    public class UpdateComentarioDto
    {
        public string? Texto { get; set; }
        public int? Estrellas { get; set; }
    }
}
