using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("Texto")]
        public required string Texto  { get; set; }
    }
}