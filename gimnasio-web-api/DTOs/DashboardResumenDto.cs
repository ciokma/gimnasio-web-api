namespace gimnasio_web_api.DTOs
{
    public class DashboardResumenDto
    {
        public int Usuarios { get; set; }
        public int UsuariosDiff { get; set; }
        public decimal Ingresos { get; set; }
        public decimal IngresosDiff { get; set; }
        public decimal Ventas { get; set; }
        public decimal VentasDiff { get; set; }
        public int Asistencias { get; set; }
        public int AsistenciasDiff { get; set; }
    }
}