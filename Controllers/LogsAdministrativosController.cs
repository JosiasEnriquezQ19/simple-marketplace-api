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
    public class LogsAdministrativosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public LogsAdministrativosController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.LogsAdministrativos.ToListAsync();
            return Ok(_mapper.Map<List<LogAdministrativoDto>>(items));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearLogAdministrativoDto dto)
        {
            var log = new LogAdministrativo
            {
                AdminId = dto.AdminId,
                Accion = dto.Accion,
                Detalles = dto.Detalles,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                FechaRegistro = DateTime.UtcNow
            };
            _db.LogsAdministrativos.Add(log);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = log.LogId }, _mapper.Map<LogAdministrativoDto>(log));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var log = await _db.LogsAdministrativos.FindAsync(id);
            if (log == null) return NotFound();
            return Ok(_mapper.Map<LogAdministrativoDto>(log));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var log = await _db.LogsAdministrativos.FindAsync(id);
            if (log == null) return NotFound();
            _db.LogsAdministrativos.Remove(log);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
