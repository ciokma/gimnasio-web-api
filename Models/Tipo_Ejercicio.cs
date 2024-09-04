using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace gimnasio_web_api.Models
{
    [Table("tipoejercicio")]
    public class Tipo_Ejercicio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Descripcion { get; set; }

        [Required]
        public int Costo { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}
