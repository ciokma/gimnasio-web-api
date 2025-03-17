using System.Threading.Tasks;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Repositories
{
    public interface IAsistenciaRepository
    {
        Task<AsistenciaDto?> ObtenerUsuarioInfoAsync(int id);
        Task<AsistenciaDto> ObtenerUltimaInformacionPagoAsync(int usuarioId);
        Task AddAsync(Asistencia asistencia);
        Task UpdateAsync(Asistencia asistencia);
        Task <IEnumerable<Asistencia>> GetAsistenciaPorFechaAsync(DateTime primerafecha, DateTime? segundafecha = null);
    }
}