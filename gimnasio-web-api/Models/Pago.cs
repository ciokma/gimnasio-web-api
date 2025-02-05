using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Models
{
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodigoPago { get; set; }

        [ForeignKey("CodigoUsuario")]
        public Usuarios? Usuario {get; set;}
        public int CodigoUsuario { get; set; }

        public int MesesPagados { get; set; }

        public int MesesPagadosA { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Monto { get; set; }

        [StringLength(50)]
        public string DetallePago { get; set; } = string.Empty;
        public bool IntervaloPago { get; set; } = false;
    }
}