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
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public FacturasController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var f = await _db.Facturas
                .Include(x => x.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FacturaId == id);

            if (f == null) return NotFound();
            return Ok(_mapper.Map<FacturaDto>(f));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Facturas
                .Include(x => x.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .AsNoTracking()
                .ToListAsync();

            return Ok(_mapper.Map<List<FacturaDto>>(list));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var f = await _db.Facturas.FindAsync(id);
            if (f == null) return NotFound();
            _db.Facturas.Remove(f);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
