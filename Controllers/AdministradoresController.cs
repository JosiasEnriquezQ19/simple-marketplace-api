using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;
using SimpleMarketplace.Api.Services;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministradoresController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public AdministradoresController(ApplicationDbContext db, IMapper mapper, IAuthService authService)
        {
            _db = db;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dtos = await _db.Administradores
                .AsNoTracking()
                .Where(a => a.Estado != "eliminado")
                .ProjectTo<AdministradorDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _db.Administradores
                .AsNoTracking()
                .Where(a => a.AdminId == id && a.Estado != "eliminado")
                .ProjectTo<AdministradorDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearAdministradorDto dto)
        {
            var admin = new Administrador
            {
                Email = dto.Email,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                NivelAcceso = dto.NivelAcceso,
                FechaCreacion = DateTime.UtcNow,
                FechaUltimoAcceso = null,
                Estado = "activo"
            };
            admin.ContrasenaHash = _authService.HashPassword(dto.Password);

            _db.Administradores.Add(admin);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = admin.AdminId }, _mapper.Map<AdministradorDto>(admin));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _db.Administradores.FindAsync(id);
            if (admin == null) return NotFound();
            if (admin.Estado == "eliminado") return NotFound();
            admin.Estado = "eliminado";
            admin.FechaUltimoAcceso = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearAdministradorDto dto)
        {
            var admin = await _db.Administradores.FindAsync(id);
            if (admin == null) return NotFound();
            if (admin.Estado == "eliminado") return NotFound();

            admin.Email = dto.Email;
            admin.Nombre = dto.Nombre;
            admin.Apellido = dto.Apellido;
            admin.NivelAcceso = dto.NivelAcceso;
            if (!string.IsNullOrEmpty(dto.Password)) admin.ContrasenaHash = _authService.HashPassword(dto.Password);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] CrearAdministradorDto dto)
        {
            var admin = await _db.Administradores.FindAsync(id);
            if (admin == null) return NotFound();
            if (admin.Estado == "eliminado") return NotFound();

            if (!string.IsNullOrEmpty(dto.Email)) admin.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Nombre)) admin.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) admin.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.NivelAcceso)) admin.NivelAcceso = dto.NivelAcceso;
            if (!string.IsNullOrEmpty(dto.Password)) admin.ContrasenaHash = _authService.HashPassword(dto.Password);

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
