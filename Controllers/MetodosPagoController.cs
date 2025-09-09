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
    public class MetodosPagoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public MetodosPagoController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.MetodosPago
                .ProjectTo<MetodoPagoDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _db.MetodosPago
                .Where(m => m.MetodoPagoId == id)
                .ProjectTo<MetodoPagoDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            // Validación de claims desactivada para pruebas

            var list = await _db.MetodosPago.Where(m => m.UsuarioId == usuarioId).ToListAsync();
            return Ok(_mapper.Map<List<MetodoPagoDto>>(list));
        }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearMetodoPagoDto dto)
        {
            // Validación de claims desactivada para pruebas

            var m = new MetodoPago
            {
                UsuarioId = dto.UsuarioId,
                TipoTarjeta = dto.TipoTarjeta,
                UltimosCuatroDigitos = dto.UltimosCuatroDigitos,
                MesExpiracion = dto.MesExpiracion,
                AñoExpiracion = dto.AñoExpiracion,
                EsPrincipal = dto.EsPrincipal
            };
            _db.MetodosPago.Add(m);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = m.MetodoPagoId }, _mapper.Map<MetodoPagoDto>(m));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearMetodoPagoDto dto)
        {
            var m = await _db.MetodosPago.FindAsync(id);
            if (m == null) return NotFound();

            m.TipoTarjeta = dto.TipoTarjeta;
            m.UltimosCuatroDigitos = dto.UltimosCuatroDigitos;
            m.MesExpiracion = dto.MesExpiracion;
            m.AñoExpiracion = dto.AñoExpiracion;
            m.EsPrincipal = dto.EsPrincipal;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateMetodoPagoDto dto)
        {
            var m = await _db.MetodosPago.FindAsync(id);
            if (m == null) return NotFound();

            if (dto.TipoTarjeta != null) m.TipoTarjeta = dto.TipoTarjeta;
            if (dto.UltimosCuatroDigitos != null) m.UltimosCuatroDigitos = dto.UltimosCuatroDigitos;
            if (dto.MesExpiracion.HasValue) m.MesExpiracion = dto.MesExpiracion.Value;
            if (dto.AñoExpiracion.HasValue) m.AñoExpiracion = dto.AñoExpiracion.Value;
            if (dto.EsPrincipal.HasValue) m.EsPrincipal = dto.EsPrincipal.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var m = await _db.MetodosPago.FindAsync(id);
                if (m == null) return NotFound();
                _db.MetodosPago.Remove(m);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log error to help debugging
                // Note: ILogger<MetodosPagoController> was not injected; use simple console log for now.
                Console.Error.WriteLine($"Error deleting MetodoPago {id}: {ex}");
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
