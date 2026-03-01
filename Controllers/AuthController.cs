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

            var token = _authService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = token,
                Usuario = _mapper.Map<UsuarioDto>(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return CreatedAtAction(nameof(GetMe), new { id = user.UsuarioId }, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();
            
            // Verificar que el usuario tenga contraseña (no sea usuario de Google)
            if (string.IsNullOrEmpty(user.ContrasenaHash))
            {
                return BadRequest(new { message = "Esta cuenta usa autenticación de Google" });
            }
            
            if (!_authService.VerifyPassword(dto.Password, user.ContrasenaHash)) return Unauthorized();

            // Generar token JWT
            var token = _authService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = token,
                Usuario = _mapper.Map<UsuarioDto>(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(response);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            // Verificar el token de Google
            var payload = await _authService.VerifyGoogleTokenAsync(dto.IdToken);
            if (payload == null)
            {
                return Unauthorized(new { message = "Token de Google inválido" });
            }

            // Buscar usuario por GoogleId o Email
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject || u.Email == payload.Email);

            if (user == null)
            {
                // Crear nuevo usuario si no existe
                user = new Usuario
                {
                    Email = payload.Email,
                    GoogleId = payload.Subject,
                    Provider = "google",
                    Nombre = payload.GivenName ?? "Usuario",
                    Apellido = payload.FamilyName ?? "Google",
                    ProfilePictureUrl = payload.Picture,
                    ContrasenaHash = null, // No necesita contraseña para usuarios de Google
                    Estado = "activo",
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                };

                _db.Usuarios.Add(user);
                await _db.SaveChangesAsync();

                var token = _authService.GenerateJwtToken(user);
                var response = new AuthResponseDto
                {
                    Token = token,
                    Usuario = _mapper.Map<UsuarioDto>(user),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return CreatedAtAction(nameof(GetMe), new { id = user.UsuarioId }, response);
            }
            else
            {
                // Actualizar GoogleId si el usuario existe pero no tenía GoogleId
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = payload.Subject;
                    user.Provider = "google";
                    user.ProfilePictureUrl = payload.Picture;
                    user.FechaActualizacion = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }

                var token = _authService.GenerateJwtToken(user);
                var response = new AuthResponseDto
                {
                    Token = token,
                    Usuario = _mapper.Map<UsuarioDto>(user),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return Ok(response);
            }
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            var admin = await _db.Administradores
                .FirstOrDefaultAsync(a => a.Email == dto.Email && a.Estado != "eliminado");

            if (admin == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            if (!_authService.VerifyPassword(dto.Password, admin.ContrasenaHash))
                return Unauthorized(new { message = "Credenciales incorrectas" });

            // Update last access
            admin.FechaUltimoAcceso = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var token = _authService.GenerateAdminJwtToken(admin);

            return Ok(new
            {
                token,
                admin = _mapper.Map<AdministradorDto>(admin),
                role = "admin",
                expiresAt = DateTime.UtcNow.AddMinutes(60)
            });
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
