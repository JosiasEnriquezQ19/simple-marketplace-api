namespace SimpleMarketplace.Api.DTOs
{
    public class FacturaDto
    {
        public int FacturaId { get; set; }
        public int PedidoId { get; set; }
        public string NumeroFactura { get; set; } = null!;
        public System.DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string EstadoPago { get; set; } = null!;
    // Incluir datos del pedido (detalles + productos) para que el frontend pueda renderizar la factura completa
    public PedidoDto? Pedido { get; set; }
    }
}
