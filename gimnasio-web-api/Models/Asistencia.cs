using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace gimnasio_web_api.Models
{
    [Table("asistencia")]
    public class Asistencia
    {
        [Key]
        public int Codigo { get; set; }
        [Required]
        public int CodigoUsuario { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public TimeSpan Hora { get; set; }
    }
}