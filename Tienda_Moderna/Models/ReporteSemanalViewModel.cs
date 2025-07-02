namespace Tienda_Moderna.Models
{
    public class ReporteSemanalViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalPorProducto { get; set; }
        public decimal Ganancia { get; set; }
    }
}