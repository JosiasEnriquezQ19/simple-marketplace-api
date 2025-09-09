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
    public class ConfiguracionesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ConfiguracionesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.Configuraciones
                .AsNoTracking()
                .ProjectTo<ConfiguracionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearConfiguracionDto dto)
        {
            var cfg = new Configuracion
            {
                Clave = dto.Clave,
                Valor = dto.Valor,
                Descripcion = dto.Descripcion,
                FechaActualizacion = DateTime.UtcNow
            };
            _db.Configuraciones.Add(cfg);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = cfg.ConfigId }, _mapper.Map<ConfiguracionDto>(cfg));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _db.Configuraciones
                .AsNoTracking()
                .Where(c => c.ConfigId == id)
                .ProjectTo<ConfiguracionDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearConfiguracionDto dto)
        {
            var cfg = await _db.Configuraciones.FindAsync(id);
            if (cfg == null) return NotFound();
            cfg.Clave = dto.Clave;
            cfg.Valor = dto.Valor;
            cfg.Descripcion = dto.Descripcion;
            cfg.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cfg = await _db.Configuraciones.FindAsync(id);
            if (cfg == null) return NotFound();
            _db.Configuraciones.Remove(cfg);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
