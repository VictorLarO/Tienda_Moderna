using System.ComponentModel.DataAnnotations;

namespace Tienda_Moderna.Models
{
    public class Marca
    {
        [Key] // Indica que IdMarca es la clave primaria
        public short id_marca { get; set; } // SMALLINT en SQL Server
        [StringLength(50)]
        public string nombre_marca { get; set; } // VARCHAR(50) en SQL Server
    }
}