using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Moderna.Models
{
    public class Corte
    {
        [Key]
        public int IdCorte { get; set; }

        [Required]
        public DateTime FechaCorte { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal MontoCorte { get; set; }

        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}