using System;
using System.Collections.Generic;

namespace SimpleMarketplace.Api.DTOs
{
    public class PedidoConUsuarioDto
    {
        public int? UsuarioId { get; set; }
        public int DireccionId { get; set; }
        public int? MetodoPagoId { get; set; }
        public List<PedidoItemDto> Items { get; set; } = new List<PedidoItemDto>();
    }
}
