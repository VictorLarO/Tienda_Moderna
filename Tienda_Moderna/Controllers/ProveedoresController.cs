using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Extensions.Logging;

namespace Tienda_Moderna.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(TiendaModernaContext context, ILogger<ProveedoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Proveedores
        public async Task<IActionResult> Index()
        {
            var proveedores = await _context.Cat_Proveedores.ToListAsync();
            if (proveedores == null || !proveedores.Any())
            {
                _logger.LogWarning("No se encontraron proveedores en la tabla cat_Proveedores.");
                ViewData["Mensaje"] = "No se encontraron proveedores.";
            }
            return View(proveedores);
        }

        // GET: /api/Proveedores
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> GetProveedores()
        {
            var proveedores = await _context.Cat_Proveedores.ToListAsync();
            if (proveedores == null || !proveedores.Any())
            {
                _logger.LogWarning("No se encontraron proveedores en la tabla cat_Proveedores.");
                return NotFound("No se encontraron proveedores.");
            }

            foreach (var p in proveedores)
                _logger.LogInformation($"ID: {p.id_prov}, Nombre: {p.nombre_prov}, Número: {p.Numero_prov}");

            return Ok(proveedores);
        }

        // POST: /Proveedores/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Proveedor creado con ID {id}", proveedor.id_prov);
                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
                return BadRequest(new { errors = new { nombre_prov = new[] { "Error al crear el proveedor: " + ex.Message } } });
            }
        }

        // POST: /Proveedores/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(short id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.id_prov)
                return BadRequest(new { errors = new { id_prov = new[] { "ID de proveedor no coincide" } } });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Proveedor actualizado con ID {id}", id);
                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor");
                return BadRequest(new { errors = new { nombre_prov = new[] { "Error al actualizar el proveedor: " + ex.Message } } });
            }
        }

        // POST: /Proveedores/Delete
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] short id)
        {
            try
            {
                var proveedor = await _context.Cat_Proveedores.FindAsync(id);
                if (proveedor == null)
                    return NotFound();

                _context.Cat_Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Proveedor eliminado con ID {id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor");
                return BadRequest(new { errors = new { id_prov = new[] { "Error al eliminar el proveedor: " + ex.Message } } });
            }
        }
    }
}
