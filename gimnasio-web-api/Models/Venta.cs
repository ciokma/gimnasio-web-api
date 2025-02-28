using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.Models
{
    [Table("venta")]
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo_venta { get; set; }

        [Required(ErrorMessage = "El nombre del vendedor es obligatorio")]

        [StringLength(30)]
        public string Nombre_vendedor { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        public int Art_vendidos { get; set; }

        [Required]
        public DateTime Fecha_venta { get; set; }

        [ForeignKey("Producto")]
        public int CodigoProducto { get; set; }

        public Producto? Producto { get; set; }
    }
}