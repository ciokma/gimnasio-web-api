using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace gimnasio_web_api.Models
{
    [Table("tipo_pagos")]
    public class Tipo_Pagos
    {
        [Key]
        [StringLength(2)]
        public string CodigoPago { get; set; }

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Monto { get; set; }
    }
}
