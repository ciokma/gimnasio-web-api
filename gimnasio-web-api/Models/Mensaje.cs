using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace gimnasio_web_api.Models
{
    [Table("mensajes")]
    public class Mensaje
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Required]
        [StringLength(50)]
        [Column("Mensaje")]
        public required string Texto  { get; set; }
    }
}