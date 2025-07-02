using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Moderna.Models
{
    public class Inventario
    {
        [Key] // Asumimos que ID_Producto actúa como clave (puede variar según la vista)
        public short ID_Producto { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre_Producto { get; set; }
        public short? ID_Marca { get; set; } // Nullable por el LEFT JOIN


        [StringLength(50)] // Ajusta según el tamaño real de Nombre_marca
        public string Marca { get; set; }

        public short? ID_Proveedor { get; set; } // Nullable por el LEFT JOIN

        [Required]
        public int Stock_Disponible { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio_Compra { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha_Caducidad { get; set; } // Nullable por el LEFT JOIN

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio_Venta { get; set; }
    }
}