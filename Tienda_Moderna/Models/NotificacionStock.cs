namespace Tienda_Moderna.Models
{
    public class NotificacionStock
    {
        public short id_prod { get; set; }
        public string nombre_prod { get; set; }
        public int? stock_prod { get; set; }
        public string mensaje { get; set; }
    }
}