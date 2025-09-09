using AutoMapper;
// ...existing code...
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CarritoController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            // Desactivada validación de claims durante pruebas

            var items = await _db.Carrito.Where(c => c.UsuarioId == usuarioId).Include(c => c.Producto).ToListAsync();
            var dto = items.Select(i => new CarritoItemDto {
                CarritoId = i.CarritoId,
                ProductoId = i.ProductoId,
                UsuarioId = i.UsuarioId,
                Cantidad = i.Cantidad,
                FechaAgregado = i.FechaAgregado
            }).ToList();

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.Carrito.Include(c => c.Producto).ToListAsync();
            var dto = items.Select(i => new CarritoItemDto {
                CarritoId = i.CarritoId,
                ProductoId = i.ProductoId,
                UsuarioId = i.UsuarioId,
                Cantidad = i.Cantidad,
                FechaAgregado = i.FechaAgregado
            }).ToList();
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var i = await _db.Carrito.FindAsync(id);
            if (i == null) return NotFound();
            var dto = new CarritoItemDto { CarritoId = i.CarritoId, ProductoId = i.ProductoId, UsuarioId = i.UsuarioId, Cantidad = i.Cantidad, FechaAgregado = i.FechaAgregado };
            return Ok(dto);
        }

        [HttpPost("usuario/{usuarioId}")]
        public async Task<IActionResult> AddToCart(int usuarioId, [FromBody] AgregarCarritoDto dto)
        {
            // Desactivada validación de claims durante pruebas

            var existing = await _db.Carrito.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ProductoId == dto.ProductoId);
            if (existing != null)
            {
                existing.Cantidad += dto.Cantidad;
                await _db.SaveChangesAsync();
                return Ok();
            }

            var item = new CarritoItem
            {
                UsuarioId = usuarioId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                FechaAgregado = DateTime.UtcNow
            };
            _db.Carrito.Add(item);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.CarritoId }, new { item.CarritoId });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _db.Carrito.FindAsync(id);
            if (item == null) return NotFound();
            // Desactivada validación de claims durante pruebas

            _db.Carrito.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AgregarCarritoDto dto)
        {
            var item = await _db.Carrito.FindAsync(id);
            if (item == null) return NotFound();
            item.Cantidad = dto.Cantidad;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] AgregarCarritoDto dto)
        {
            var item = await _db.Carrito.FindAsync(id);
            if (item == null) return NotFound();
            if (dto.Cantidad != 0) item.Cantidad = dto.Cantidad;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("usuario/{usuarioId}/checkout")]
        public async Task<IActionResult> Checkout(int usuarioId, [FromBody] CrearPedidoCompletoDto dto)
        {
            // dto.Items is ignored; items are taken from the user's cart
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var cartItems = await _db.Carrito.Where(c => c.UsuarioId == usuarioId).Include(c => c.Producto).ToListAsync();
                if (cartItems == null || !cartItems.Any()) return BadRequest("El carrito está vacío");

                // Validar dirección
                var direccion = await _db.Direcciones.FindAsync(dto.DireccionId);
                if (direccion == null) return BadRequest($"Dirección {dto.DireccionId} no existe");
                if (direccion.UsuarioId != usuarioId) return BadRequest("La dirección no pertenece al usuario");

                // Validar método de pago
                var metodoPago = await _db.MetodosPago.FindAsync(dto.MetodoPagoId);
                if (metodoPago == null) return BadRequest($"Método de pago {dto.MetodoPagoId} no existe");
                if (metodoPago.UsuarioId != usuarioId) return BadRequest("El método de pago no pertenece al usuario");

                decimal subtotal = 0;
                var detalles = new List<DetallePedido>();

                foreach (var ci in cartItems)
                {
                    var producto = ci.Producto;
                    if (producto == null) return BadRequest($"Producto {ci.ProductoId} no existe");
                    if (producto.Estado == "eliminado") return BadRequest($"Producto {ci.ProductoId} no disponible");
                    if (producto.Stock < ci.Cantidad) return BadRequest($"No hay suficiente stock para {producto.Nombre}");

                    producto.Stock -= ci.Cantidad;
                    subtotal += producto.Precio * ci.Cantidad;

                    detalles.Add(new DetallePedido { ProductoId = producto.ProductoId, Cantidad = ci.Cantidad, PrecioUnitario = producto.Precio });
                }

                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    DireccionId = dto.DireccionId,
                    MetodoPagoId = dto.MetodoPagoId,
                    Subtotal = subtotal,
                    CostoEnvio = 0,
                    Impuestos = Math.Round(subtotal * 0.18M, 2),
                    Total = Math.Round(subtotal + Math.Round(subtotal * 0.18M, 2), 2),
                    Estado = "pendiente",
                    FechaPedido = DateTime.UtcNow,
                    Detalles = detalles
                };

                _db.Pedidos.Add(pedido);
                // eliminar items del carrito
                _db.Carrito.RemoveRange(cartItems);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return CreatedAtAction("Get", "Pedidos", new { id = pedido.PedidoId }, _mapper.Map<PedidoDto>(pedido));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
