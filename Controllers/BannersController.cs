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
    public class BannersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public BannersController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? soloActivos)
        {
            var query = _db.Banners.AsQueryable();
            if (soloActivos.HasValue && soloActivos.Value)
            {
                query = query.Where(b => b.Activo);
            }
            
            var banners = await query.OrderBy(b => b.Orden).ToListAsync();
            return Ok(_mapper.Map<List<BannerDto>>(banners));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _db.Banners.FindAsync(id);
            if (banner == null) return NotFound();
            return Ok(_mapper.Map<BannerDto>(banner));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearBannerDto dto)
        {
            var banner = _mapper.Map<Banner>(dto);
            _db.Banners.Add(banner);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = banner.BannerId }, _mapper.Map<BannerDto>(banner));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearBannerDto dto)
        {
            var banner = await _db.Banners.FindAsync(id);
            if (banner == null) return NotFound();

            _mapper.Map(dto, banner);
            banner.FechaActualizacion = DateTime.UtcNow;
            
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _db.Banners.FindAsync(id);
            if (banner == null) return NotFound();

            _db.Banners.Remove(banner);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
