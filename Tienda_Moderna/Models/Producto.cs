using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Moderna.Models
{
    public class Producto
    {
        [Key]
        public short id_prod { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre_prod { get; set; }

        [Required]
        public int stock_prod { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_ven_prod { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_com_prod { get; set; }

        [DataType(DataType.Date)]
        public DateTime? fecha_cad_prod { get; set; }

        // Claves foráneas
        [ForeignKey(nameof(Proveedor))]
        public short? id_prov_prod { get; set; }

        [ForeignKey(nameof(Marca))]
        public short? id_marca_prod { get; set; }

        // Propiedades de navegación
        [NotMapped]
        public virtual Proveedor proveedor { get; set; }

        [NotMapped]
        public virtual Marca marca { get; set; }
    }
}
