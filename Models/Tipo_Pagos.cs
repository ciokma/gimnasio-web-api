using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.Models
{
    [Table("tipo_pagos")]
    public class Tipo_Pagos
    {
        [Key]
        [StringLength(2)]
        [Required]
        public string CodigoPago { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Monto { get; set; }
    }
}
