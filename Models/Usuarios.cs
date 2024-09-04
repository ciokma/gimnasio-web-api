using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace gimnasio_web_api.Models
{
    [Table("usuarios")]
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; }

        public string? Foto { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaIngreso { get; set; }

        [Required]
        public bool Activo { get; set; }

        [StringLength(150)]
        public string? Observaciones { get; set; }

        public ICollection<Fechas_Usuario>? FechasUsuario { get; set; }

        public Usuarios()
        {
            Nombres = string.Empty;
            Apellidos = string.Empty;
            Telefono = string.Empty;
        }
    }
}
