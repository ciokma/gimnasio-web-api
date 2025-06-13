using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace gimnasio_web_api.Models
{
    public class ConfiguracionesSistema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }

        public int? Valor { get; set; }
        [Required]
        public bool Estado { get; set; } = false;
        [StringLength(255)]
        public required string Descripcion { get; set; }
    }
}