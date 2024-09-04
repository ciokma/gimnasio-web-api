using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("usuarios")]
public class Usuarios
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Codigo { get; set; }

    [JsonRequired]
    [StringLength(50)]
    public required string Nombres { get; set; }

    [JsonRequired]
    [StringLength(50)]
    public required string Apellidos { get; set; }

    [JsonRequired]
    [StringLength(15)]
    public required string Telefono { get; set; }

    public string? Foto { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaIngreso { get; set; }

    [JsonRequired]
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
