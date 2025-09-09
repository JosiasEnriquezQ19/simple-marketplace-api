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
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UsuariosController(ApplicationDbContext db, IMapper mapper, IAuthService authService)
        {
            _db = db;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Usuarios
                .AsNoTracking()
                .Where(u => u.Estado != "eliminado")
                .ProjectTo<UsuarioDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Direcciones)
                .Include(u => u.MetodosPago)
                .Include(u => u.Pedidos).ThenInclude(p => p.Detalles).ThenInclude(d => d.Producto)
                .Where(u => u.UsuarioId == id && u.Estado != "eliminado")
                .SingleOrDefaultAsync();

            if (entity == null) return NotFound();

            var dto = _mapper.Map<UsuarioDetailDto>(entity);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearUsuarioDto dto)
        {
            if (await _db.Usuarios.AnyAsync(x => x.Email == dto.Email))
                return BadRequest(new { message = "Email ya registrado" });

            var user = _mapper.Map<Usuario>(dto);
            user.ContrasenaHash = _authService.HashPassword(dto.Password);
            user.FechaCreacion = DateTime.UtcNow;
            user.FechaActualizacion = DateTime.UtcNow;
            user.Estado = "activo";

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            var entity = await _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Direcciones)
                .Include(u => u.MetodosPago)
                .Include(u => u.Pedidos).ThenInclude(p => p.Detalles).ThenInclude(d => d.Producto)
                .SingleOrDefaultAsync(u => u.UsuarioId == user.UsuarioId);

            var resultDto = _mapper.Map<UsuarioDetailDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = user.UsuarioId }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            if (u.Estado == "eliminado") return NotFound();

            if (!string.IsNullOrEmpty(dto.Nombre)) u.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) u.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono)) u.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Estado)) u.Estado = dto.Estado;
            if (!string.IsNullOrEmpty(dto.Password)) u.ContrasenaHash = _authService.HashPassword(dto.Password);

            u.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            if (u.Estado == "eliminado") return NotFound();
            // soft-delete
            u.Estado = "eliminado";
            u.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("verificar-email")]
        public async Task<IActionResult> VerificarEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "El email es requerido" });

            bool existeEmail = await _db.Usuarios
                .AnyAsync(u => u.Email == email && u.Estado != "eliminado");

            return Ok(existeEmail);
        }

        [HttpGet("verificar-telefono")]
        public async Task<IActionResult> VerificarTelefono([FromQuery] string telefono)
        {
            if (string.IsNullOrEmpty(telefono))
                return BadRequest(new { message = "El teléfono es requerido" });

            bool existeTelefono = await _db.Usuarios
                .AnyAsync(u => u.Telefono == telefono && u.Estado != "eliminado");

            return Ok(existeTelefono);
        }
    }
}
