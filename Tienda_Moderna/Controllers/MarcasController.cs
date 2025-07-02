using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Extensions.Logging;

namespace Tienda_Moderna.Controllers
{
    public class MarcasController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<MarcasController> _logger;

        public MarcasController(TiendaModernaContext context, ILogger<MarcasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Marcas
        public async Task<IActionResult> Index()
        {
            var marcas = await _context.Cat_Marcas.ToListAsync();
            if (marcas == null || !marcas.Any())
            {
                _logger.LogWarning("No se encontraron marcas en la tabla cat_Marcas.");
                ViewData["Mensaje"] = "No se encontraron marcas.";
            }
            return View(marcas);
        }

        // GET: /api/Marcas
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.Cat_Marcas.ToListAsync();
            if (marcas == null || !marcas.Any())
            {
                _logger.LogWarning("No se encontraron marcas en la tabla cat_Marcas.");
                return NotFound("No se encontraron marcas.");
            }

            foreach (var marca in marcas)
                _logger.LogInformation($"ID: {marca.id_marca}, Nombre: {marca.nombre_marca}");

            return Ok(marcas);
        }

        // POST: /Marcas/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Marca marca)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Add(marca);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Marca creada con ID {id}", marca.id_marca);
                return Ok(marca);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear marca");
                return BadRequest(new { errors = new { nombre_marca = new[] { "Error al crear la marca: " + ex.Message } } });
            }
        }

        // POST: /Marcas/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(short id, [FromBody] Marca marca)
        {
            if (id != marca.id_marca)
                return BadRequest(new { errors = new { id_marca = new[] { "ID de marca no coincide" } } });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Update(marca);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Marca actualizada con ID {id}", id);
                return Ok(marca);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar marca");
                return BadRequest(new { errors = new { nombre_marca = new[] { "Error al actualizar la marca: " + ex.Message } } });
            }
        }

        // POST: /Marcas/Delete
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] short id)
        {
            try
            {
                var marca = await _context.Cat_Marcas.FindAsync(id);
                if (marca == null)
                    return NotFound();

                _context.Cat_Marcas.Remove(marca);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Marca eliminada con ID {id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar marca");
                return BadRequest(new { errors = new { id_marca = new[] { "Error al eliminar la marca: " + ex.Message } } });
            }
        }
    }
}
