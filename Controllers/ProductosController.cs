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
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductosController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? categoria)
        {
            // Mostrar todos los productos, incluidos los ocultos
            var q = _db.Productos.AsNoTracking();
            if (!string.IsNullOrEmpty(categoria)) q = q.Where(p => p.Categoria == categoria);
            
            // Cargar los productos primero, luego mapear para evitar problemas de traducción con Imagenes
            var productos = await q.ToListAsync();
            var list = productos.Select(p => _mapper.Map<ProductoDto>(p)).ToList();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var producto = await _db.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null) return NotFound();
            
            var dto = _mapper.Map<ProductoDto>(producto);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearProductoDto dto)
        {
            var prod = _mapper.Map<Producto>(dto);
            prod.FechaCreacion = DateTime.UtcNow;
            prod.FechaActualizacion = DateTime.UtcNow;
            _db.Productos.Add(prod);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = prod.ProductoId }, _mapper.Map<ProductoDto>(prod));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoDto dto)
        {
            var prod = await _db.Productos.FindAsync(id);
            if (prod == null) return NotFound();

            // Validate Estado if provided to avoid writing invalid ENUM values
            var allowedStates = new[] { "disponible", "agotado", "descontinuado", "oculto" };
            if (!string.IsNullOrEmpty(dto.Estado) && !allowedStates.Contains(dto.Estado))
            {
                return BadRequest(new { message = "Estado inválido. Valores permitidos: " + string.Join(",", allowedStates) });
            }

            // Actualizar campos solo si están presentes en el DTO
            if (dto.Nombre != null) prod.Nombre = dto.Nombre;
            if (dto.Descripcion != null) prod.Descripcion = dto.Descripcion;
            if (dto.Precio.HasValue) prod.Precio = dto.Precio.Value;
            if (dto.Stock.HasValue) prod.Stock = dto.Stock.Value;

            // Manejar URLs de imágenes - tratar cadenas vacías como null
            if (dto.ImagenUrl != null) prod.ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null! : dto.ImagenUrl;
            if (dto.ImagenUrl2 != null) prod.ImagenUrl2 = string.IsNullOrWhiteSpace(dto.ImagenUrl2) ? null : dto.ImagenUrl2;
            if (dto.ImagenUrl3 != null) prod.ImagenUrl3 = string.IsNullOrWhiteSpace(dto.ImagenUrl3) ? null : dto.ImagenUrl3;
            if (dto.ImagenUrl4 != null) prod.ImagenUrl4 = string.IsNullOrWhiteSpace(dto.ImagenUrl4) ? null : dto.ImagenUrl4;
            if (dto.ImagenUrl5 != null) prod.ImagenUrl5 = string.IsNullOrWhiteSpace(dto.ImagenUrl5) ? null : dto.ImagenUrl5;
            if (dto.ImagenUrl6 != null) prod.ImagenUrl6 = string.IsNullOrWhiteSpace(dto.ImagenUrl6) ? null : dto.ImagenUrl6;
            if (dto.ImagenUrl7 != null) prod.ImagenUrl7 = string.IsNullOrWhiteSpace(dto.ImagenUrl7) ? null : dto.ImagenUrl7;
            if (!string.IsNullOrEmpty(dto.Categoria)) prod.Categoria = dto.Categoria!;
            if (!string.IsNullOrEmpty(dto.Estado)) prod.Estado = dto.Estado!;

            prod.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateProductoDto dto)
        {
            var prod = await _db.Productos.FindAsync(id);
            if (prod == null) return NotFound();

            var allowedStates = new[] { "disponible", "agotado", "descontinuado", "oculto" };

            // Aplicar solo campos presentes
            if (dto.Nombre != null) prod.Nombre = dto.Nombre;
            if (dto.Descripcion != null) prod.Descripcion = dto.Descripcion;
            if (dto.Precio.HasValue) prod.Precio = dto.Precio.Value;
            if (dto.Stock.HasValue) prod.Stock = dto.Stock.Value;
            
            // Tratar cadenas vacías como null para las URLs de imágenes
            if (dto.ImagenUrl != null) prod.ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null! : dto.ImagenUrl;
            if (dto.ImagenUrl2 != null) prod.ImagenUrl2 = string.IsNullOrWhiteSpace(dto.ImagenUrl2) ? null : dto.ImagenUrl2;
            if (dto.ImagenUrl3 != null) prod.ImagenUrl3 = string.IsNullOrWhiteSpace(dto.ImagenUrl3) ? null : dto.ImagenUrl3;
            if (dto.ImagenUrl4 != null) prod.ImagenUrl4 = string.IsNullOrWhiteSpace(dto.ImagenUrl4) ? null : dto.ImagenUrl4;
            if (dto.ImagenUrl5 != null) prod.ImagenUrl5 = string.IsNullOrWhiteSpace(dto.ImagenUrl5) ? null : dto.ImagenUrl5;
            if (dto.ImagenUrl6 != null) prod.ImagenUrl6 = string.IsNullOrWhiteSpace(dto.ImagenUrl6) ? null : dto.ImagenUrl6;
            if (dto.ImagenUrl7 != null) prod.ImagenUrl7 = string.IsNullOrWhiteSpace(dto.ImagenUrl7) ? null : dto.ImagenUrl7;
            if (dto.Categoria != null) prod.Categoria = dto.Categoria;
            if (dto.Estado != null)
            {
                if (!allowedStates.Contains(dto.Estado))
                    return BadRequest(new { message = "Estado inválido. Valores permitidos: " + string.Join(",", allowedStates) });
                prod.Estado = dto.Estado;
            }

            prod.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _db.Productos.FindAsync(id);
            if (prod == null) return NotFound();
            // soft-delete: use 'oculto' which exists in DB enum
            prod.Estado = "oculto";
            prod.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}