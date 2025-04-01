using System.ComponentModel.DataAnnotations.Schema;
namespace gimnasio_web_api.Models
{
    public class Backup
    {
        public int Id { get; set; }
        [Column("proximo_respaldo")]
        public DateTime ProximoRespaldo { get; set; }
        [Column("frecuencia_respaldo")]
        public required string FrecuenciaRespaldo { get; set; }
        [Column("fecha_respaldoanterior")]
        public DateTime? FechaRespaldoAnterior { get; set; }
    }
}