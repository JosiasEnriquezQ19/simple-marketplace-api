using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMarketplace.Api.Data;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/productos/{productoId}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ComentariosController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/productos/{productoId}/comentarios
        [HttpGet]
        public async Task<IActionResult> GetComentarios(int productoId)
        {
            try
            {
                var comentarios = await _db.Comentarios
                    .Include(c => c.Usuario) // Incluir la relación con Usuario
                    .Where(c => c.ProductoId == productoId)
                    .OrderByDescending(c => c.FechaComentario)
                    .Select(c => new
                    {
                        comentarioId = c.ComentarioId,
                        productoId = c.ProductoId,
                        usuarioId = c.UsuarioId,
                        usuarioNombre = c.Usuario.Nombre + " " + c.Usuario.Apellido, // Nombre completo del usuario
                        texto = c.Texto,
                        estrellas = c.Estrellas,
                        fechaComentario = c.FechaComentario
                    })
                    .ToListAsync();

                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener comentarios", detalle = ex.Message });
            }
        }

        // POST: api/productos/{productoId}/comentarios
        [HttpPost]
        public async Task<IActionResult> CrearComentario(int productoId, [FromBody] CrearComentarioRequest request)
        {
            try
            {
                // Validaciones básicas
                if (request == null)
                    return BadRequest(new { error = "Datos del comentario son requeridos" });

                if (string.IsNullOrWhiteSpace(request.Texto))
                    return BadRequest(new { error = "El texto del comentario es requerido" });

                if (request.Estrellas < 1 || request.Estrellas > 5)
                    return BadRequest(new { error = "Las estrellas deben estar entre 1 y 5" });

                // Verificar que el producto existe
                var productoExiste = await _db.Productos.AnyAsync(p => p.ProductoId == productoId);
                if (!productoExiste)
                    return NotFound(new { error = "Producto no encontrado" });

                // Crear el comentario
                var comentario = new Comentario
                {
                    ProductoId = productoId,
                    UsuarioId = request.UsuarioId ?? 1, // Si no se proporciona, usar 1 por defecto
                    Texto = request.Texto,
                    Estrellas = request.Estrellas,
                    FechaComentario = DateTime.Now
                };

                _db.Comentarios.Add(comentario);
                await _db.SaveChangesAsync();

                // Retornar el comentario creado
                var resultado = new
                {
                    comentarioId = comentario.ComentarioId,
                    productoId = comentario.ProductoId,
                    usuarioId = comentario.UsuarioId,
                    texto = comentario.Texto,
                    estrellas = comentario.Estrellas,
                    fechaComentario = comentario.FechaComentario
                };

                return CreatedAtAction(nameof(GetComentarios), new { productoId }, resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al crear comentario", detalle = ex.Message });
            }
        }

        // GET: api/productos/{productoId}/comentarios/promedio
        [HttpGet("promedio")]
        public async Task<IActionResult> GetPromedioEstrellas(int productoId)
        {
            try
            {
                var comentarios = await _db.Comentarios
                    .Where(c => c.ProductoId == productoId)
                    .ToListAsync();

                if (!comentarios.Any())
                {
                    return Ok(new { promedio = 0, totalComentarios = 0 });
                }

                var promedio = comentarios.Average(c => c.Estrellas);
                var total = comentarios.Count;

                return Ok(new { promedio = Math.Round(promedio, 1), totalComentarios = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al calcular promedio", detalle = ex.Message });
            }
        }
    }

    // Clase simple para la request
    public class CrearComentarioRequest
    {
        public string Texto { get; set; } = "";
        public int Estrellas { get; set; }
        public int? UsuarioId { get; set; }
    }
}
