using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace gimnasio_web_api.DTOs;

public class UsuariosInactivosDto
{
    public int Codigo { get; set; }
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
    public string? Foto { get; set; }
    public bool Activo { get; set; }
    public DateTime? FechaPago { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}