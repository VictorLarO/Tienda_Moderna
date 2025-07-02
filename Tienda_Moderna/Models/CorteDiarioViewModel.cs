using System;
using System.Collections.Generic;

namespace Tienda_Moderna.Models
{
    public class CorteDiarioViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal TotalVenta { get; set; }
        public string MetodoPagoVenta { get; set; }
        public string NombreCli { get; set; }
        public short IdCorte { get; set; }
        public DateTime FechaCorte { get; set; }
        public string NombreCorte { get; set; }
    }

    public class CorteDiarioResponse
    {
        public List<CorteDiarioViewModel> Data { get; set; }
        public string Error { get; set; }
    }
}