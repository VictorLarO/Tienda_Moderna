namespace Tienda_Moderna.Models
{
    public class TicketEncabezado
    {
        public int id_venta { get; set; }
        public DateTime fecha_venta { get; set; }
        public string nombre_cli { get; set; }
        public string metodo_pago_venta { get; set; }
    }

    public class TicketDetalle
    {
        public string producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
    }

    public class TicketTotal
    {
        public decimal total { get; set; }
    }

    public class TicketResult
    {
        public TicketEncabezado Encabezado { get; set; } = new TicketEncabezado();
        public List<TicketDetalle> Detalle { get; set; } = new List<TicketDetalle>();
        public TicketTotal Total { get; set; } = new TicketTotal();
    }
}