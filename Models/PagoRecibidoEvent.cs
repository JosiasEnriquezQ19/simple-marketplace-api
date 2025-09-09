namespace SimpleMarketplace.Api.Models
{
    public class PagoRecibidoEvent
    {
        public int PedidoId { get; set; }
        public int FacturaId { get; set; }
        public string NumeroFactura { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
