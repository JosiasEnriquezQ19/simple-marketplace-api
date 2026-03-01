using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CategoriasController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? estado)
        {
            var query = _db.Categorias.AsNoTracking();
            
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(c => c.Estado == estado);
            }

            var categorias = await query.OrderBy(c => c.Nombre).ToListAsync();
            var list = _mapper.Map<List<CategoriaDto>>(categorias);
            return Ok(list);
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categoria = await _db.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            var dto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(dto);
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto)
        {
            // Verificar si ya existe una categoría con ese nombre
            var existe = await _db.Categorias.AnyAsync(c => c.Nombre == dto.Nombre);
            if (existe)
                return BadRequest(new { message = "Ya existe una categoría con ese nombre" });

            var categoria = _mapper.Map<Categoria>(dto);
            categoria.FechaCreacion = DateTime.UtcNow;
            categoria.FechaActualizacion = DateTime.UtcNow;
            
            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return CreatedAtAction(nameof(Get), new { id = categoria.CategoriaId }, categoriaDto);
        }

        // PUT: api/Categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto dto)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            // Si se está cambiando el nombre, verificar que no exista otro con ese nombre
            if (!string.IsNullOrEmpty(dto.Nombre) && dto.Nombre != categoria.Nombre)
            {
                var existe = await _db.Categorias.AnyAsync(c => c.Nombre == dto.Nombre && c.CategoriaId != id);
                if (existe)
                    return BadRequest(new { message = "Ya existe otra categoría con ese nombre" });
                
                categoria.Nombre = dto.Nombre;
            }

            if (dto.Descripcion != null)
                categoria.Descripcion = dto.Descripcion;

            if (dto.ImagenUrl != null)
                categoria.ImagenUrl = dto.ImagenUrl;

            if (!string.IsNullOrEmpty(dto.Estado))
                categoria.Estado = dto.Estado;

            categoria.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(categoriaDto);
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            // Verificar si tiene productos asociados
            if (categoria.Productos.Any())
            {
                return BadRequest(new { 
                    message = "No se puede eliminar la categoría porque tiene productos asociados",
                    productosCount = categoria.Productos.Count
                });
            }

            _db.Categorias.Remove(categoria);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Categoría eliminada correctamente" });
        }

        // PATCH: api/Categorias/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] UpdateEstadoRequest request)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            categoria.Estado = request.Estado;
            categoria.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Estado actualizado correctamente", estado = categoria.Estado });
        }

        // GET: api/Categorias/5/productos
        [HttpGet("{id}/productos")]
        public async Task<IActionResult> GetProductosPorCategoria(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Productos)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            var productos = _mapper.Map<List<ProductoDto>>(categoria.Productos);
            return Ok(new 
            { 
                categoriaId = categoria.CategoriaId,
                categoriaNombre = categoria.Nombre,
                productos = productos,
                total = productos.Count
            });
        }
    }

    // Clase auxiliar para actualizar estado
    public class UpdateEstadoRequest
    {
        public string Estado { get; set; } = null!;
    }
}
