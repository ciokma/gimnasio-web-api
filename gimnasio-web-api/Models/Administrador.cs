using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.Models
{
    [Table("administrador")]
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Usuario { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Clave { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = null!;

        [Required]
        [Column("FechaIngreso")]
        public DateTime FechaIngreso { get; set; }

        [Required]
        public bool Activo { get; set; } = false;
    }
}