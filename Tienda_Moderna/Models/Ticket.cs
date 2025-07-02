
    public class TicketResult
    {
        public Encabezado Encabezado { get; set; } = new Encabezado();
        public List<TicketDetalle> Detalle { get; set; } = new List<TicketDetalle>();
        public Total Total { get; set; } = new Total();
    }

    public class Encabezado
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

    public class Total
    {
        public decimal total { get; set; }
    }

