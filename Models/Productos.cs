using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("productos")]
public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodigoProducto { get; set; }
    
    [JsonRequired]
    [StringLength(50)]
    public required string Descripcion { get; set; }

    [JsonRequired]
    [Column(TypeName = "decimal(10,2)")]
    public required decimal Precio { get; set; }

    [JsonRequired]
    public required double Existencias { get; set; }
}
