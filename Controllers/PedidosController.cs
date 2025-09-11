using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public PedidosController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        {
            try
            {
                // Cargar el pedido con todas sus relaciones
                var pedido = await _db.Pedidos
                    .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                    .Include(p => p.Usuario)
                    .Include(p => p.Direccion)
                    .FirstOrDefaultAsync(p => p.PedidoId == id && p.Estado != "eliminado");
                    
                if (pedido == null) return NotFound();
                
                // Mapear manualmente en lugar de usar ProjectTo
                var pedidoDto = _mapper.Map<PedidoDto>(pedido);
                return Ok(pedidoDto);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? usuarioId)
        {
            try
            {
                var q = _db.Pedidos.AsQueryable();
                q = q.Where(p => p.Estado != "eliminado");
                if (usuarioId.HasValue) q = q.Where(p => p.UsuarioId == usuarioId.Value);

                // Cargar las entidades con sus relaciones
                var pedidos = await q
                    .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                    .Include(p => p.Usuario)
                    .Include(p => p.Direccion)
                    .ToListAsync();
                    
                var list = pedidos.Select(p => _mapper.Map<PedidoDto>(p)).ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                // Devolver una lista vacía en caso de error para evitar bloqueos en el frontend
                return Ok(new List<PedidoDto>());
            }
        }

    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            // Devuelve únicamente los pedidos asociados al usuario (excluye eliminados)
            var pedidos = await _db.Pedidos
                .Where(p => p.UsuarioId == usuarioId && p.Estado != "eliminado")
                .ToListAsync();
                
            var list = pedidos.Select(p => _mapper.Map<PedidoDto>(p)).ToList();
            return Ok(list);
        }

    // GET: /api/pedidos/with-user?usuarioId=5
    [HttpGet("with-user")]
    public async Task<IActionResult> GetWithUser([FromQuery] int? usuarioId)
    {
        try 
        {
            var q = _db.Pedidos.AsQueryable();
            q = q.Where(p => p.Estado != "eliminado");
            if (usuarioId.HasValue) q = q.Where(p => p.UsuarioId == usuarioId.Value);

            var pedidos = await q
                .Include(p => p.Usuario)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .Include(p => p.Direccion)
                .ToListAsync();

            var dtos = pedidos.Select(p => _mapper.Map<PedidoWithUsuarioDto>(p)).ToList();
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            // Devolver una lista vacía en caso de error para evitar bloqueos en el frontend
            return Ok(new List<PedidoWithUsuarioDto>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateFromDto([FromBody] PedidoConUsuarioDto dto)
    {
        if (dto == null || !dto.UsuarioId.HasValue)
        {
            return BadRequest("Se requiere UsuarioId en el cuerpo de la solicitud");
        }

        // Extraer el DTO interno o crear uno compatible
        var pedidoDto = new CrearPedidoCompletoDto
        {
            DireccionId = dto.DireccionId,
            MetodoPagoId = dto.MetodoPagoId,
            Items = dto.Items ?? new List<PedidoItemDto>()
        };

        // Redirigir al método existente que ya tiene toda la lógica
        return await Create(dto.UsuarioId.Value, pedidoDto);
    }

        [HttpPost("usuario/{usuarioId}")]
        public async Task<IActionResult> Create(int usuarioId, [FromBody] CrearPedidoCompletoDto dto)
        {
            // Validación de claims desactivada para pruebas
            // Validaciones simples y transacción para decrementar stock
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Validar dirección
                var direccion = await _db.Direcciones.FindAsync(dto.DireccionId);
                if (direccion == null) return BadRequest($"Dirección {dto.DireccionId} no existe");
                if (direccion.UsuarioId != usuarioId) return BadRequest("La dirección no pertenece al usuario");

                // Validar método de pago (solo si se proporcionó uno)
                // Tratar valores especiales (0 o negativos) como null para compatibilidad con frontend
                if (dto.MetodoPagoId <= 0)
                {
                    dto.MetodoPagoId = null; // Convertir explícitamente a null
                }
                
                if (dto.MetodoPagoId.HasValue)
                {
                    var metodoPago = await _db.MetodosPago.FindAsync(dto.MetodoPagoId);
                    if (metodoPago == null) return BadRequest($"Método de pago {dto.MetodoPagoId} no existe");
                    if (metodoPago.UsuarioId != usuarioId) return BadRequest("El método de pago no pertenece al usuario");
                }
                // Si no se proporciona método de pago, continuamos sin validación adicional

                decimal subtotal = 0;
                var detalles = new List<DetallePedido>();
                foreach (var item in dto.Items)
                {
                    var producto = await _db.Productos.FindAsync(item.ProductoId);
                    if (producto == null) return BadRequest($"Producto {item.ProductoId} no existe");
                    if (producto.Estado == "eliminado") return BadRequest($"Producto {item.ProductoId} no disponible");
                    if (producto.Stock < item.Cantidad) return BadRequest($"No hay suficiente stock para {producto.Nombre}");

                    producto.Stock -= item.Cantidad;
                    subtotal += producto.Precio * item.Cantidad;

                    detalles.Add(new DetallePedido { ProductoId = producto.ProductoId, Cantidad = item.Cantidad, PrecioUnitario = producto.Precio });
                }

                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    DireccionId = dto.DireccionId,
                    MetodoPagoId = dto.MetodoPagoId,
                    Subtotal = subtotal,
                    CostoEnvio = 0,
                    Impuestos = 0, // Impuesto eliminado (antes era 18%)
                    Total = subtotal, // Total sin impuesto
                    Estado = "pendiente",
                    FechaPedido = DateTime.UtcNow,
                    Detalles = detalles
                };

                _db.Pedidos.Add(pedido);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                // Recargar el pedido con Detalles -> Producto y Usuario para devolver el DTO completo
                var created = await _db.Pedidos
                    .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedido.PedidoId);

                // ...

                return CreatedAtAction(nameof(Get), new { id = pedido.PedidoId }, _mapper.Map<PedidoDto>(created ?? pedido));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchPedidoDto dto)
        {
            var pedido = await _db.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            // Allowed values must match the DB ENUM for Pedidos.estado
            var allowedStates = new[] { "pendiente", "procesando", "enviado", "entregado", "cancelado" };

            var changed = false;
            var requireAtomicSave = false;

            if (!string.IsNullOrEmpty(dto.Estado))
            {
                if (!allowedStates.Contains(dto.Estado))
                {
                    return BadRequest(new { message = "Estado inválido. Valores permitidos: " + string.Join(", ", allowedStates) });
                }

                // Guardar estado anterior para posibles efectos (por ejemplo, devolver stock si se cancela
                // o volver a descontar stock si se reactiva)
                var estadoAnterior = pedido.Estado;

                if (pedido.Estado != dto.Estado)
                {
                    // Cargar detalles y productos asociados para poder ajustar stock si es necesario
                    var pedidoConDetalles = await _db.Pedidos
                        .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                        .FirstOrDefaultAsync(p => p.PedidoId == id);

                    // Si vamos a cancelar: devolver stock
                    if (dto.Estado == "cancelado" && estadoAnterior != "cancelado" && estadoAnterior != "eliminado")
                    {
                        if (pedidoConDetalles?.Detalles != null)
                        {
                            foreach (var detalle in pedidoConDetalles.Detalles)
                            {
                                if (detalle.Producto != null)
                                {
                                    detalle.Producto.Stock += detalle.Cantidad;
                                }
                            }
                            requireAtomicSave = true;
                        }
                        pedido.Estado = dto.Estado;
                        changed = true;
                    }

                    // Si vamos a reactivar desde "cancelado"/"eliminado" a un estado activo, intentar descontar stock
                    var estadosActivos = new[] { "pendiente", "procesando", "enviado", "entregado" };
                    if (estadosActivos.Contains(dto.Estado) && (estadoAnterior == "cancelado" || estadoAnterior == "eliminado"))
                    {
                        if (pedidoConDetalles?.Detalles == null)
                        {
                            return BadRequest(new { message = "No se encontraron los detalles del pedido para reactivar." });
                        }

                        // Validar que hay stock suficiente para cada producto antes de descontar
                        var faltantes = new List<string>();
                        foreach (var detalle in pedidoConDetalles.Detalles)
                        {
                            if (detalle.Producto == null)
                            {
                                faltantes.Add($"Producto desconocido (detalle {detalle.DetallePedidoId})");
                                continue;
                            }
                            if (detalle.Producto.Stock < detalle.Cantidad)
                            {
                                faltantes.Add($"{detalle.Producto.Nombre} (disponible: {detalle.Producto.Stock}, requerido: {detalle.Cantidad})");
                            }
                        }

                        if (faltantes.Count > 0)
                        {
                            return BadRequest(new { message = "Stock insuficiente para reactivar pedido: " + string.Join("; ", faltantes) });
                        }

                        // Descontar stock ahora que validamos
                        foreach (var detalle in pedidoConDetalles.Detalles)
                        {
                            if (detalle.Producto != null)
                            {
                                detalle.Producto.Stock -= detalle.Cantidad;
                            }
                        }
                        requireAtomicSave = true;
                        pedido.Estado = dto.Estado;
                        changed = true;
                    }

                    // Si no aplica devolución ni re-descontar, solo cambiar estado
                    if (!changed)
                    {
                        pedido.Estado = dto.Estado;
                        changed = true;
                    }
                }
            }

            if (dto.NumeroSeguimiento != null && pedido.NumeroSeguimiento != dto.NumeroSeguimiento)
            {
                pedido.NumeroSeguimiento = dto.NumeroSeguimiento;
                changed = true;
            }

            if (changed)
            {
                // optional timestamp field on Pedido entity — create if missing
                try
                {
                    pedido.FechaPedido = pedido.FechaPedido; // preserve FechaPedido
                }
                catch { }
                // update a modification timestamp if your entity has one; otherwise keep as-is
                // If you have FechaActualizacion on Pedido, update it here (many entities do)
                var prop = pedido.GetType().GetProperty("FechaActualizacion");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(pedido, DateTime.UtcNow);
                }

                await _db.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            // Cargar el pedido con sus detalles y productos
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);
            if (pedido == null) return NotFound();

            // Devolver stock solo si el pedido no estaba ya eliminado/cancelado
            if (pedido.Estado != "eliminado" && pedido.Estado != "cancelado")
            {
                foreach (var detalle in pedido.Detalles)
                {
                    if (detalle.Producto != null)
                    {
                        detalle.Producto.Stock += detalle.Cantidad;
                    }
                }
            }

            // soft-delete: marcar como eliminado
            pedido.Estado = "eliminado";
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/pagar")]
        public async Task<IActionResult> Pagar(int id)
        {
            try
            {
                var pedido = await _db.Pedidos.Include(p => p.Detalles).FirstOrDefaultAsync(p => p.PedidoId == id);
                if (pedido == null) return NotFound();
                if (pedido.Estado == "cancelado" || pedido.Estado == "eliminado") return BadRequest("El pedido fue cancelado o eliminado");
                if (pedido.Estado == "pagado") return BadRequest("El pedido ya fue pagado");

                // Simular cobro exitoso
                // Nota: la columna 'estado' en la tabla Pedidos no contiene 'pagado' en su ENUM,
                // por eso usamos un valor existente ('procesando') aquí. Si prefieres 'pagado',
                // actualiza el ENUM en la BD con ALTER TABLE para añadir 'pagado'.
                pedido.Estado = "procesando";

                // Crear factura
                var factura = new Factura
                {
                    PedidoId = pedido.PedidoId,
                    NumeroFactura = $"F-{DateTime.UtcNow:yyyyMMddHHmmss}-{pedido.PedidoId}",
                    FechaEmision = DateTime.UtcNow,
                    Subtotal = pedido.Subtotal,
                    Impuestos = pedido.Impuestos,
                    Total = pedido.Total,
                    EstadoPago = "pagado"
                };

                _db.Facturas.Add(factura);
                await _db.SaveChangesAsync();

                // Recargar la factura como entidad (incluyendo pedido, detalles y producto)
                var createdFacturaEntity = await _db.Facturas
                    .Include(f => f.Pedido).ThenInclude(p => p.Detalles).ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(f => f.FacturaId == factura.FacturaId);

                if (createdFacturaEntity != null)
                {
                    var dto = _mapper.Map<FacturaDto>(createdFacturaEntity);
                    return CreatedAtAction("Get", "Facturas", new { id = dto.FacturaId }, dto);
                }

                // Fallback: mapear la factura creada (sin navegación)
                return CreatedAtAction("Get", "Facturas", new { id = factura.FacturaId }, _mapper.Map<FacturaDto>(factura));
            }
            catch (Exception ex)
            {
                return Problem(detail: $"Error al procesar el pago: {ex.Message}", statusCode: 500);
            }
        }
    }
}
