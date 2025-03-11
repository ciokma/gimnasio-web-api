namespace gimnasio_web_api.DTOs
{
    public class AsistenciaDto
    {
        public UsuarioDto? Usuario { get; set; }
        public FechasUsuarioDto? UltimaFechaUsuario { get; set; }
        public PagoDto? UltimoPago { get; set; }
    }
}