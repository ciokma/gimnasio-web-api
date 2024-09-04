using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("tipoejercicio")]
public class Tipo_Ejercicio
{
    [Key]
    [JsonRequired]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Codigo { get; set; }

    [JsonRequired]
    [MaxLength(50)]
    public required string Descripcion { get; set; }

    [JsonRequired]
    public required int Costo { get; set; }

    [JsonRequired]
    public required bool Activo { get; set; }
}
