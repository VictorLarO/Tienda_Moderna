using System.ComponentModel.DataAnnotations;

namespace Tienda_Moderna.Models
{
    public class Cliente
    {
        [Key] // Indica que IdMarca es la clave primaria
        public short id_cli { get; set; } // SMALLINT en SQL Server
        [Required]
        [StringLength(100)]
        public string nombre_cli { get; set; }

        [StringLength(100)]
        public string Numero_cli { get; set; }

        [StringLength(100)]
        public string dir_cli { get; set; }
    }
}