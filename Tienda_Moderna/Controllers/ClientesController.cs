using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Extensions.Logging;

namespace Tienda_Moderna.Controllers
{
    public class ClientesController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(TiendaModernaContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Vista de listado tradicional
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Cat_Clientes.ToListAsync();
            if (!clientes.Any())
                ViewData["Mensaje"] = "No se encontraron clientes.";

            // Aquí decimos explícitamente "use Views/Clientes/Index.cshtml"
            return View("Index", clientes);
        }

        

        // API: GET /api/Clientes
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Cat_Clientes.ToListAsync();
            if (clientes == null || !clientes.Any())
            {
                _logger.LogWarning("No se encontraron clientes en la tabla cat_Clientes.");
                return NotFound("No se encontraron clientes.");
            }

            foreach (var cliente in clientes)
            {
                _logger.LogInformation($"ID: {cliente.id_cli}, Nombre: {cliente.nombre_cli}, Numero: {cliente.Numero_cli}, Direccion: {cliente.dir_cli}");
            }

            return Ok(clientes);
        }

        // POST: /Clientes/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente creado con ID {id}", cliente.id_cli);
                    return Ok(cliente);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el cliente.");
                    return BadRequest(new { errors = new { nombre_cli = new[] { "Error al crear el cliente: " + ex.Message } } });
                }
            }
            return BadRequest(ModelState);
        }

        // POST: /Clientes/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(short id, [FromBody] Cliente cliente)
        {
            if (id != cliente.id_cli)
            {
                return BadRequest(new { errors = new { id_cli = new[] { "ID de cliente no coincide" } } });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente actualizado con ID {id}", cliente.id_cli);
                    return Ok(cliente);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el cliente.");
                    return BadRequest(new { errors = new { nombre_cli = new[] { "Error al actualizar el cliente: " + ex.Message } } });
                }
            }

            return BadRequest(ModelState);
        }

        // POST: /Clientes/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] short id)
        {
            try
            {
                var cliente = await _context.Cat_Clientes.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                _context.Cat_Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cliente eliminado con ID {id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente.");
                return BadRequest(new { errors = new { id_cli = new[] { "Error al eliminar el cliente: " + ex.Message } } });
            }
        }
    }
}
