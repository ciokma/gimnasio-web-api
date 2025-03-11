using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.Models
{
    [Table("administrador")]
    public class Administrador
    {
        [Key]
        [StringLength(30)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string Clave { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string Apellido { get; set; } = null!;

        [Required]
        public DateTime Fecha_ingreso { get; set; }

        [Required]
        public bool Activo { get; set; } = false;

        [Required]
        [StringLength(50)]
        public string Direccion { get; set; } = null!;
    }
}
