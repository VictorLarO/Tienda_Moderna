using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tienda_Moderna.Controllers
{
    public class ProductosController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(TiendaModernaContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Ruta para la vista frontal
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Cat_Productos.ToListAsync();
            if (productos == null || !productos.Any())
            {
                _logger.LogWarning("No se encontraron productos en la tabla Cat_Productos.");
                return NotFound();
            }
            return View(productos);
        }

        // Ruta para la API
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Cat_Productos.ToListAsync();
            if (productos == null || !productos.Any())
            {
                _logger.LogWarning("No se encontraron productos en la tabla Cat_Productos.");
                return NotFound("No se encontraron productos.");
            }

            foreach (var producto in productos)
            {
                _logger.LogInformation($"ID: {producto.id_prod}, Nombre: {producto.nombre_prod}, Stock: {producto.stock_prod}");
            }

            return Ok(productos);
        }

        // Acción para la búsqueda de productos
        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string buscar)
        {
            _logger.LogInformation($"Parámetro de búsqueda recibido: '{buscar}' (Longitud: {(buscar?.Length ?? 0)})");

            List<Producto> resultados;

            if (string.IsNullOrWhiteSpace(buscar))
            {
                _logger.LogInformation("El campo de búsqueda está vacío. No se devuelven productos.");
                resultados = new List<Producto>();
            }
            else
            {
                _logger.LogInformation($"Buscando productos con el término: '{buscar}'");
                resultados = await _context.Cat_Productos
                    .Where(p => p.nombre_prod.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();
            }

            if (resultados.Any())
            {
                _logger.LogInformation($"Se encontraron {resultados.Count} productos.");
                foreach (var producto in resultados)
                {
                    _logger.LogInformation($"Producto encontrado: ID: {producto.id_prod}, Nombre: {producto.nombre_prod}");
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron productos en la búsqueda.");
            }

            return View("BuscarProductos", resultados);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Create([FromBody] Producto producto)
        {
            _logger.LogInformation("Datos recibidos en Create: {@Producto}", producto);

            ModelState.Remove("Marca");
            ModelState.Remove("Proveedor");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                _logger.LogWarning("Errores de validación en Create: {@Errors}", errors);
                return BadRequest(new { errors });
            }

            if (producto.id_prov_prod.HasValue && !await _context.Cat_Proveedores.AnyAsync(p => p.id_prov == producto.id_prov_prod))
            {
                _logger.LogWarning($"Proveedor con ID {producto.id_prov_prod} no encontrado.");
                return BadRequest(new { errors = new { id_prov_prod = new[] { "El proveedor especificado no existe." } } });
            }
            if (producto.id_marca_prod.HasValue && !await _context.Cat_Marcas.AnyAsync(m => m.id_marca == producto.id_marca_prod))
            {
                _logger.LogWarning($"Marca con ID {producto.id_marca_prod} no encontrada.");
                return BadRequest(new { errors = new { id_marca_prod = new[] { "La marca especificada no existe." } } });
            }

            producto.id_prod = 0;

            try
            {
                _context.Cat_Productos.Add(producto);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Producto creado: ID {producto.id_prod}, Nombre: {producto.nombre_prod}");
                return Ok(new { id_prod = producto.id_prod });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar producto en la base de datos.");
                return BadRequest(new { errors = new { general = new[] { "Error al guardar en la base de datos. Verifique los datos." } } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear producto.");
                return StatusCode(500, new { errors = new { general = new[] { "Error inesperado en el servidor." } } });
            }
        }

        // POST: /Productos/Edit/{id}
        [HttpPost]
        [Route("Productos/Edit/{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Edit(short id, [FromBody] Producto producto)
        {
            _logger.LogInformation("Datos recibidos en Edit: {@Producto}", producto);

            if (id != producto.id_prod)
            {
                _logger.LogWarning($"ID en URL ({id}) no coincide con id_prod ({producto.id_prod})");
                return BadRequest(new { errors = new { id_prod = new[] { "El ID del producto no coincide con el registro a editar." } } });
            }

            var existingProduct = await _context.Cat_Productos.FindAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning($"Producto con ID {id} no encontrado.");
                return NotFound(new { errors = new { id_prod = new[] { "Producto no encontrado." } } });
            }

            ModelState.Remove("Marca");
            ModelState.Remove("Proveedor");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                _logger.LogWarning("Errores de validación en Edit: {@Errors}", errors);
                return BadRequest(new { errors });
            }

            if (producto.id_prov_prod.HasValue && !await _context.Cat_Proveedores.AnyAsync(p => p.id_prov == producto.id_prov_prod))
            {
                _logger.LogWarning($"Proveedor con ID {producto.id_prov_prod} no encontrado.");
                return BadRequest(new { errors = new { id_prov_prod = new[] { "El proveedor especificado no existe." } } });
            }
            if (producto.id_marca_prod.HasValue && !await _context.Cat_Marcas.AnyAsync(m => m.id_marca == producto.id_marca_prod))
            {
                _logger.LogWarning($"Marca con ID {producto.id_marca_prod} no encontrada.");
                return BadRequest(new { errors = new { id_marca_prod = new[] { "La marca especificada no existe." } } });
            }

            try
            {
                existingProduct.nombre_prod = producto.nombre_prod;
                existingProduct.stock_prod = producto.stock_prod;
                existingProduct.precio_com_prod = producto.precio_com_prod;
                existingProduct.precio_ven_prod = producto.precio_ven_prod;
                existingProduct.fecha_cad_prod = producto.fecha_cad_prod;
                existingProduct.id_prov_prod = producto.id_prov_prod;
                existingProduct.id_marca_prod = producto.id_marca_prod;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Producto actualizado: ID {existingProduct.id_prod}, Nombre: {existingProduct.nombre_prod}");
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar producto.");
                return Conflict(new { errors = new { general = new[] { "El producto fue modificado por otro usuario. Por favor, recargue y reintente." } } });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al actualizar producto en la base de datos.");
                return BadRequest(new { errors = new { general = new[] { "Error al guardar en la base de datos. Verifique los datos." } } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar producto.");
                return StatusCode(500, new { errors = new { general = new[] { "Error inesperado en el servidor." } } });
            }
        }
        public async Task<IActionResult> Editar()
        {
            var productos = await _context.Cat_Productos.ToListAsync();
            var proveedoresIds = await _context.Cat_Proveedores.Select(p => p.id_prov).ToListAsync();
            var marcasIds = await _context.Cat_Marcas.Select(m => m.id_marca).ToListAsync();
            ViewBag.ProveedoresIds = proveedoresIds;
            ViewBag.MarcasIds = marcasIds;
            return View("~/Views/Inventario/Editar.cshtml", productos);
        }
    }
}