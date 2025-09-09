namespace SimpleMarketplace.Api.DTOs
{
    public class PedidoDto
    {
        public int PedidoId { get; set; }
        public int UsuarioId { get; set; }
    
        public int DireccionId { get; set; }
        public int? MetodoPagoId { get; set; } // Ahora es opcional
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = null!;
        public System.DateTime FechaPedido { get; set; }
        public string? NumeroSeguimiento { get; set; }
        public List<DetallePedidoDto> Detalles { get; set; } = new List<DetallePedidoDto>();
    }

    public class CrearPedidoCompletoDto
    {
        public int DireccionId { get; set; }
        public int? MetodoPagoId { get; set; } // Ahora es opcional (puede ser null)
        public List<PedidoItemDto> Items { get; set; } = new List<PedidoItemDto>();
    }

    public class PedidoItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class PatchPedidoDto
    {
        public string? Estado { get; set; }
        public string? NumeroSeguimiento { get; set; }
    }

    // DTO para listar pedidos junto al usuario asociado (uso en panel admin)
    public class PedidoWithUsuarioDto
    {
        public int PedidoId { get; set; }
        public int UsuarioId { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public int DireccionId { get; set; }
        public int? MetodoPagoId { get; set; } // Ahora es opcional
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = null!;
        public System.DateTime FechaPedido { get; set; }
        public string? NumeroSeguimiento { get; set; }
        public List<DetallePedidoDto> Detalles { get; set; } = new List<DetallePedidoDto>();
    }
}
