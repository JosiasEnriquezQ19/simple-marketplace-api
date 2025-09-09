namespace SimpleMarketplace.Api.DTOs
{
    public class DetallePedidoDto
    {
        public int DetallePedidoId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public ProductoDto? Producto { get; set; }
    }
}
