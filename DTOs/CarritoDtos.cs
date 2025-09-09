namespace SimpleMarketplace.Api.DTOs
{
    public class CarritoItemDto
    {
        public int CarritoId { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public System.DateTime FechaAgregado { get; set; }
        public ProductoDto? Producto { get; set; }
    }

    public class AgregarCarritoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
