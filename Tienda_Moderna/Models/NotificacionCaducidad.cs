namespace Tienda_Moderna.Models
{
    public class NotificacionCaducidad
    {
        public short id_prod { get; set; }
        public string nombre_prod { get; set; }
        public DateTime? fecha_cad { get; set; } // Alias usado en el procedimiento
        public int? dias_restantes { get; set; }
        public string mensaje { get; set; }
    }
}