using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Moderna.Models
{
    public class Venta
    {
        [Key]
        public short IdVenta { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal TotalVenta { get; set; }

        public short? IdCliVenta { get; set; }

        [Required]
        [StringLength(10)]
        public string MetodoPagoVenta { get; set; }

        [ForeignKey("IdCliVenta")]
        public Cliente? Cliente { get; set; }
    }
}