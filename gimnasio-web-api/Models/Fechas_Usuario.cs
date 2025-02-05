using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace gimnasio_web_api.Models
{
    [Table("fechas_usuario")]
    public class Fechas_Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Usuarios")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        
        [JsonIgnore]
        public Usuarios? Usuario { get; set; }

        [Column("Fecha_Pago", TypeName = "DATE")]
        public DateTime? FechaPago { get; set; }

        [Column("Fecha_PagoA", TypeName = "DATE")]
        public DateTime? FechaPagoA { get; set; }

        [Column("Fecha_Vencimiento", TypeName = "DATE")]
        public DateTime? FechaVencimiento { get; set; }
    }
}
