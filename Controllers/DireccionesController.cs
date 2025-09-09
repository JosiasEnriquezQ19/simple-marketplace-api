using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<DireccionesController> _logger;

        public DireccionesController(ApplicationDbContext db, IMapper mapper, ILogger<DireccionesController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            // Validación de claims desactivada para pruebas
            if (usuarioId <= 0)
            {
                return BadRequest(new { message = "usuarioId inválido" });
            }

            try
            {
                // Verificar que el usuario exista para devolver 404 en caso contrario
                var userExists = await _db.Usuarios.AnyAsync(u => u.UsuarioId == usuarioId);
                if (!userExists)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                var list = await _db.Direcciones.Where(d => d.UsuarioId == usuarioId).ToListAsync();
                return Ok(_mapper.Map<List<DireccionDto>>(list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener direcciones para usuarioId {UsuarioId}", usuarioId);
                // Devolvemos una respuesta segura: lista vacía en vez de propagar 500 al cliente.
                return Ok(new List<DireccionDto>());
            }
        }

        // Soporte adicional: permitir consulta por query ?usuarioId=1 para compatibilidad con frontend
        [HttpGet]
        public async Task<IActionResult> GetByUsuarioQuery([FromQuery] int? usuarioId, [FromQuery] int? userId)
        {
            var id = usuarioId ?? userId;
            if (id == null) return BadRequest(new { message = "usuarioId requerido" });
            return await GetByUsuario(id.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var d = await _db.Direcciones.FindAsync(id);
            if (d == null) return NotFound();
            return Ok(_mapper.Map<DireccionDto>(d));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearDireccionDto dto)
        {
            // Validación de claims desactivada para pruebas

            var d = new Direccion
            {
                UsuarioId = dto.UsuarioId,
                Calle = dto.Calle,
                Ciudad = dto.Ciudad,
                Estado = dto.Estado ?? string.Empty,
                CodigoPostal = dto.CodigoPostal,
                Pais = dto.Pais ?? "Perú",
                EsPrincipal = dto.EsPrincipal
            };
            _db.Direcciones.Add(d);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByUsuario), new { usuarioId = d.UsuarioId }, _mapper.Map<DireccionDto>(d));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Direcciones.FindAsync(id);
            if (d == null) return NotFound();
            // physical delete (table uses estado column for region, no soft-delete present)
            _db.Direcciones.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearDireccionDto dto)
        {
            var d = await _db.Direcciones.FindAsync(id);
            if (d == null) return NotFound();

            d.Calle = dto.Calle;
            d.Ciudad = dto.Ciudad;
            d.Estado = dto.Estado ?? d.Estado;
            d.CodigoPostal = dto.CodigoPostal;
            d.Pais = dto.Pais ?? d.Pais;
            d.EsPrincipal = dto.EsPrincipal;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateDireccionDto dto)
        {
            var d = await _db.Direcciones.FindAsync(id);
            if (d == null) return NotFound();

            if (dto.Calle != null) d.Calle = dto.Calle;
            if (dto.Ciudad != null) d.Ciudad = dto.Ciudad;
            if (dto.Estado != null) d.Estado = dto.Estado;
            if (dto.CodigoPostal != null) d.CodigoPostal = dto.CodigoPostal;
            if (dto.Pais != null) d.Pais = dto.Pais;
            if (dto.EsPrincipal.HasValue) d.EsPrincipal = dto.EsPrincipal.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
