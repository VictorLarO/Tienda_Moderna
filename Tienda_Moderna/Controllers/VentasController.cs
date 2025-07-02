using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Moderna.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data;

namespace Tienda_Moderna.Controllers
{
    public class VentasController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<VentasController> _logger;

        public VentasController(TiendaModernaContext context, ILogger<VentasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var inventario = await _context.ViewInventario.ToListAsync();
            return View(inventario);
        }

        [HttpPost]
        public async Task<IActionResult> ValidarVenta(string itemsJson, decimal totalFrontend)
        {
            try
            {
                _logger.LogInformation("Validando venta - itemsJson: {itemsJson}, totalFrontend: {totalFrontend}", itemsJson, totalFrontend);
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_validar_venta";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@items_json", itemsJson ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@total_frontend", totalFrontend));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int fieldCount = reader.FieldCount;
                                _logger.LogInformation("Número de columnas devueltas: {fieldCount}", fieldCount);

                                var status = reader.GetString(0); // Primera columna: status
                                var mensaje = reader.GetString(1); // Segunda columna: mensaje
                                decimal? totalReal = null;
                                // Solo intentar leer totalReal si hay al menos 3 columnas
                                if (fieldCount > 2 && !reader.IsDBNull(2))
                                {
                                    totalReal = reader.GetDecimal(2);
                                }
                                else
                                {
                                    _logger.LogWarning("Faltan columnas esperadas (total_real), fieldCount: {fieldCount}", fieldCount);
                                }

                                _logger.LogInformation("Resultado - status: {status}, mensaje: {mensaje}, totalReal: {totalReal}", status, mensaje, totalReal);
                                return Json(new { status, mensaje, totalReal });
                            }
                            _logger.LogWarning("No se obtuvo respuesta del procedimiento sp_validar_venta");
                            return Json(new { status = "error", mensaje = "No se obtuvo respuesta del procedimiento" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ValidarVenta - itemsJson: {itemsJson}, totalFrontend: {totalFrontend}", itemsJson, totalFrontend);
                return Json(new { status = "error", mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta(int? idCliente, string metodoPago, string itemsJson, decimal totalVenta)
        {
            try
            {
                _logger.LogInformation("Registrando venta - idCliente: {idCliente}, metodoPago: {metodoPago}, itemsJson: {itemsJson}, totalVenta: {totalVenta}", idCliente, metodoPago, itemsJson, totalVenta);
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_InsertarVentaCompleta";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id_cli_venta", idCliente ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@metodo_pago_venta", metodoPago ?? "EFECTIVO"));
                        command.Parameters.Add(new SqlParameter("@items_json", itemsJson ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@total_venta", totalVenta));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int fieldCount = reader.FieldCount;
                                _logger.LogInformation("Número de columnas devueltas: {fieldCount}", fieldCount);

                                var status = reader.GetString(0); // Primera columna: status
                                var mensaje = reader.GetString(1); // Segunda columna: mensaje
                                string datos = null;
                                if (fieldCount > 2 && !reader.IsDBNull(2)) // Tercera columna: datos (si existe)
                                {
                                    datos = reader.GetString(2);
                                }

                                _logger.LogInformation("Resultado - status: {status}, mensaje: {mensaje}, datos: {datos}", status, mensaje, datos);
                                return Json(new { status, mensaje, datos });
                            }
                            _logger.LogWarning("No se obtuvo respuesta del procedimiento sp_InsertarVentaCompleta");
                            return Json(new { status = "error", mensaje = "No se obtuvo respuesta del procedimiento" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la venta - idCliente: {idCliente}, metodoPago: {metodoPago}, itemsJson: {itemsJson}, totalVenta: {totalVenta}", idCliente, metodoPago, itemsJson, totalVenta);
                return Json(new { status = "error", mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ImprimirTicket(int idVenta)
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_imprimir_ticket_venta";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id_venta", idVenta));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var ticket = new TicketResult();

                            // Leer el encabezado
                            if (await reader.ReadAsync())
                            {
                                ticket.Encabezado.id_venta = reader.GetInt16(0); // Cambiado a GetInt16
                                ticket.Encabezado.fecha_venta = reader.GetDateTime(1);
                                ticket.Encabezado.nombre_cli = reader.IsDBNull(2) ? "No registrado" : reader.GetString(2);
                                ticket.Encabezado.metodo_pago_venta = reader.IsDBNull(3) ? "EFECTIVO" : reader.GetString(3);
                            }

                            // Avanzar al siguiente result set (detalle)
                            await reader.NextResultAsync();
                            while (await reader.ReadAsync())
                            {
                                ticket.Detalle.Add(new TicketDetalle
                                {
                                    producto = reader.IsDBNull(0) ? "N/A" : reader.GetString(0),
                                    cantidad = reader.GetInt32(1), // Cantidad puede ser INT, así que sigue siendo GetInt32
                                    precio_unitario = reader.GetDecimal(2),
                                    subtotal = reader.GetDecimal(3)
                                });
                            }

                            // Avanzar al siguiente result set (total)
                            await reader.NextResultAsync();
                            if (await reader.ReadAsync())
                            {
                                ticket.Total.total = reader.GetDecimal(0);
                            }

                            if (ticket.Encabezado.id_venta == 0 && ticket.Detalle.Count == 0 && ticket.Total.total == 0)
                            {
                                _logger.LogWarning("Datos insuficientes para generar el ticket de venta {idVenta}.", idVenta);
                                return Json(new { error = "Datos insuficientes para el ticket" });
                            }

                            return Json(ticket);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el ticket de venta {idVenta}.", idVenta);
                return Json(new { error = "Error interno: " + ex.Message });
            }
        }
    }
}