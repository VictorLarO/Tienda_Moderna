using System.ComponentModel.DataAnnotations;

namespace Tienda_Moderna.Models
{
    public class Proveedor
    {
        [Key] // Indica que IdMarca es la clave primaria
        public short id_prov { get; set; }

        [StringLength(100)]
        public string nombre_prov { get; set; }

        [StringLength(10)]
        public string Numero_prov { get; set; }
    }
}