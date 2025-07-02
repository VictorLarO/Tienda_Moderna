using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Tienda_Moderna.Controllers
{
    public class InventarioController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<InventarioController> _logger;

        public InventarioController(TiendaModernaContext context, ILogger<InventarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var inventario = await _context.ViewInventario.ToListAsync();
            if (inventario == null || !inventario.Any())
            {
                _logger.LogWarning("No se encontraron registros en la vista view_inventario.");
                return NotFound();
            }

            // Obtener notificaciones
            var notificacionesCaducidad = await GetNotificacionesCaducidad();
            var notificacionesStock = await GetNotificacionesStock();

            _logger.LogInformation($"Notificaciones de caducidad: {notificacionesCaducidad.Count}, Stock: {notificacionesStock.Count}");

            ViewBag.NotificacionesCaducidad = notificacionesCaducidad;
            ViewBag.NotificacionesStock = notificacionesStock;

            return View(inventario);
        }

        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> GetInventario()
        {
            var inventario = await _context.ViewInventario.ToListAsync();
            if (inventario == null || !inventario.Any())
            {
                _logger.LogWarning("No se encontraron registros en la vista view_inventario.");
                return NotFound("No se encontraron registros en el inventario.");
            }

            foreach (var item in inventario)
            {
                _logger.LogInformation($"ID Producto: {item.ID_Producto}, Nombre: {item.Nombre_Producto}, Stock: {item.Stock_Disponible}");
            }

            return Ok(inventario);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string buscar)
        {
            if (string.IsNullOrEmpty(buscar))
            {
                return View(new List<Producto>());
            }

            var productos = await _context.Cat_Productos
                .FromSqlRaw("EXEC sp_buscar_producto @buscar",
                    new SqlParameter("@buscar", buscar ?? (object)DBNull.Value))
                .ToListAsync();

            if (productos == null || !productos.Any())
            {
                _logger.LogWarning($"No se encontraron productos para la búsqueda: {buscar}");
                return View(new List<Producto>());
            }

            _logger.LogInformation($"Búsqueda exitosa para: {buscar}, encontrados {productos.Count} productos.");
            return View(productos);
        }

        private async Task<List<string>> GetNotificacionesCaducidad()
        {
            var notificaciones = new List<string>();
            try
            {
                var resultados = await _context.Set<NotificacionCaducidad>()
                    .FromSqlRaw("EXEC sp_notificar_productos_por_caducar")
                    .ToListAsync();

                notificaciones.AddRange(resultados.Select(r => r.mensaje ?? "Mensaje no disponible"));
                _logger.LogInformation($"Notificaciones de caducidad obtenidas: {notificaciones.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones de caducidad.");
                notificaciones.Add("Error al cargar notificaciones de caducidad.");
            }
            return notificaciones;
        }

        private async Task<List<string>> GetNotificacionesStock()
        {
            var notificaciones = new List<string>();
            try
            {
                var resultados = await _context.Set<NotificacionStock>()
                    .FromSqlRaw("EXEC sp_alerta_stock_bajo")
                    .ToListAsync();

                notificaciones.AddRange(resultados.Select(r => r.mensaje ?? "Mensaje no disponible"));
                _logger.LogInformation($"Notificaciones de stock obtenidas: {notificaciones.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones de stock.");
                notificaciones.Add("Error al cargar notificaciones de stock.");
            }
            return notificaciones;
        }
    }
}