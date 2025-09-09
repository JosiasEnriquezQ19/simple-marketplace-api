using AutoMapper;
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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(ApplicationDbContext db, IAuthService authService, IMapper mapper)
        {
            _db = db;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CrearUsuarioDto dto)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email ya registrado" });

            var user = _mapper.Map<Usuario>(dto);
            user.ContrasenaHash = _authService.HashPassword(dto.Password);
            user.FechaCreacion = DateTime.UtcNow;
            user.FechaActualizacion = DateTime.UtcNow;

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            var result = _mapper.Map<UsuarioDto>(user);
            return CreatedAtAction(nameof(GetMe), new { id = user.UsuarioId }, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();
            if (!_authService.VerifyPassword(dto.Password, user.ContrasenaHash)) return Unauthorized();

            // Devolver información pública del usuario (sin token) para simplificar
            return Ok(_mapper.Map<UsuarioDto>(user));
        }

        [HttpGet("me/{id}")]
        public async Task<IActionResult> GetMe(int id)
        {
            var user = await _db.Usuarios.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UsuarioDto>(user));
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateMe(int id, [FromBody] SimpleMarketplace.Api.DTOs.UpdateUsuarioDto dto)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Nombre)) u.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) u.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono)) u.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Estado)) u.Estado = dto.Estado;
            if (!string.IsNullOrEmpty(dto.Password)) u.ContrasenaHash = _authService.HashPassword(dto.Password);

            u.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Permitir PUT /api/Auth/me con body que incluya UsuarioId
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMeByBody([FromBody] SimpleMarketplace.Api.DTOs.UpdateUsuarioDto dto)
        {
            if (dto.UsuarioId == null || dto.UsuarioId <= 0)
                return BadRequest(new { message = "UsuarioId requerido en el body" });

            var u = await _db.Usuarios.FindAsync(dto.UsuarioId.Value);
            if (u == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Nombre)) u.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) u.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono)) u.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Estado)) u.Estado = dto.Estado;
            if (!string.IsNullOrEmpty(dto.Password)) u.ContrasenaHash = _authService.HashPassword(dto.Password);

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
    }
}
