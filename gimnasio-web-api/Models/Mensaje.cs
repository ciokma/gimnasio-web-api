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
        public required string Texto { get; set; }
        [Required]
        [StringLength(50)]
        public required string Emisor { get; set; }
        [Column("fechaemision", TypeName = "DATE")]
        public DateTime? FechaEmision { get; set; }
        public required bool Leido { get; set; } = true;
    }
}