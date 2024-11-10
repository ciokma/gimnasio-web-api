namespace gimnasio_web_api.DTOs
{
    public class UsuarioDto
    {
        public required int Codigo { get; set; }
        public required string Nombres { get; set; }
        public string? Apellidos { get; set; }
    }
}