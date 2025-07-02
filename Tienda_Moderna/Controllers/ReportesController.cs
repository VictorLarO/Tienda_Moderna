using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Tienda_Moderna.Models;

namespace Tienda_Moderna.Controllers
{
    public class ReportesController : Controller
    {
        private readonly TiendaModernaContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(TiendaModernaContext context, ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CorteDiario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerarCorteDiario()
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_generar_corte_diario";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var results = new List<CorteDiarioViewModel>();
                            do
                            {
                                while (await reader.ReadAsync())
                                {
                                    var item = new CorteDiarioViewModel
                                    {
                                        IdVenta = reader.IsDBNull(0) ? (short)0 : reader.GetInt16(0),
                                        FechaVenta = reader.GetDateTime(1),
                                        TotalVenta = reader.GetDecimal(2),
                                        MetodoPagoVenta = reader.IsDBNull(3) ? "EFECTIVO" : reader.GetString(3),
                                        NombreCli = reader.IsDBNull(4) ? "Sin ventas" : reader.GetString(4),
                                        IdCorte = reader.GetInt16(5),
                                        FechaCorte = reader.GetDateTime(6),
                                        NombreCorte = reader.GetString(7)
                                    };
                                    results.Add(item);
                                }
                            } while (await reader.NextResultAsync());

                            _logger.LogInformation("Datos devueltos por GenerarCorteDiario: {Count} registros", results.Count);
                            if (results.Any())
                            {
                                return Json(new CorteDiarioResponse { Data = results });
                            }
                            return Json(new CorteDiarioResponse { Error = "No se generó el corte: No hay ventas hoy o ya existe un corte." });
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                return Json(new CorteDiarioResponse { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el corte diario.");
                return Json(new CorteDiarioResponse { Error = "Error interno: " + ex.Message });
            }
        }
        // GET: Reportes/ReporteSemanal
        public IActionResult ReporteSemanal()
        {
            return View();
        }

        // POST: Reportes/ObtenerReporteSemanal
        [HttpPost]
        public async Task<IActionResult> ObtenerReporteSemanal()
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_reporte_ventas_semanal";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var results = new List<ReporteSemanalViewModel>();
                            while (await reader.ReadAsync())
                            {
                                var item = new ReporteSemanalViewModel
                                {
                                    IdVenta = reader.GetInt16(0),
                                    FechaVenta = reader.GetDateTime(1),
                                    IdProducto = reader.GetInt16(2),
                                    NombreProducto = reader.GetString(3),
                                    Cantidad = reader.GetInt32(4),
                                    TotalPorProducto = reader.GetDecimal(5),
                                    Ganancia = reader.GetDecimal(6)
                                };
                                results.Add(item);
                            }

                            _logger.LogInformation("Datos devueltos por ObtenerReporteSemanal: {Count} registros", results.Count);
                            if (results.Any())
                            {
                                return Json(new { success = true, data = results });
                            }
                            return Json(new { success = false, error = "No hay ventas registradas esta semana." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el reporte semanal.");
                return Json(new { success = false, error = "Error interno: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> MostrarCorteDiario()
        {
            try
            {
                var corteExistente = await ObtenerCorteDiarioExistente();
                if (corteExistente != null && corteExistente.Any())
                {
                    _logger.LogInformation("Datos devueltos por MostrarCorteDiario: {Count} registros", corteExistente.Count);
                    return Json(new CorteDiarioResponse { Data = corteExistente });
                }
                return Json(new CorteDiarioResponse { Error = "No se encontró un corte diario para hoy." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar el corte diario.");
                return Json(new CorteDiarioResponse { Error = "Error interno: " + ex.Message });
            }
        }
        private async Task<List<CorteDiarioViewModel>> ObtenerCorteDiarioExistente()
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    SELECT 
                        v.id_venta,
                        v.fecha_venta,
                        v.total_venta,
                        v.metodo_pago_venta,
                        c.nombre_cli,
                        v.id_corte,
                        v.fecha_corte,
                        v.nombre_corte
                    FROM view_detalle_cortes v
                    LEFT JOIN cat_clientes c ON v.id_venta IN (
                        SELECT id_venta 
                        FROM cat_Ventas 
                        WHERE id_cli_venta = c.id_cli
                    )
                    WHERE CAST(v.fecha_corte AS DATE) = CAST(GETDATE() AS DATE)";
                        command.CommandType = CommandType.Text;

                        var results = new List<CorteDiarioViewModel>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new CorteDiarioViewModel
                                {
                                    IdVenta = reader.IsDBNull(0) ? (short)0 : reader.GetInt16(0),
                                    FechaVenta = reader.GetDateTime(1),
                                    TotalVenta = reader.GetDecimal(2),
                                    MetodoPagoVenta = reader.IsDBNull(3) ? "EFECTIVO" : reader.GetString(3),
                                    NombreCli = reader.IsDBNull(4) ? "Sin ventas" : reader.GetString(4),
                                    IdCorte = reader.IsDBNull(5) ? (short)0 : reader.GetInt16(5),
                                    FechaCorte = reader.GetDateTime(6),
                                    NombreCorte = reader.GetString(7)
                                };
                                results.Add(item);
                            }
                        }
                        return results.Any() ? results : null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el corte diario existente.");
                return null;
            }
        }
    }
}