using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;

public class AsistenciaCleanupService
{
    private readonly IAsistenciaRepository _asistenciaRepository;
    private readonly ILogger<AsistenciaCleanupService> _logger;

    public AsistenciaCleanupService(IAsistenciaRepository asistenciaRepository, ILogger<AsistenciaCleanupService> logger)
    {
        _asistenciaRepository = asistenciaRepository;
        _logger = logger;
    }

    public async Task EjecutarLimpiezaAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando limpieza de registros de asistencia...");

            var añosConAsistencias = await _asistenciaRepository.GetAñosConAsistenciasAsync();
            var añosOrdenados = añosConAsistencias.Select(a => a.Año).OrderByDescending(a => a).ToList();

            var mesesConRegistros = new List<(int Año, int Mes)>();

            foreach (var año in añosOrdenados)
            {
                var meses = await _asistenciaRepository.GetMesesConAsistenciasAsync(año);
                mesesConRegistros.AddRange(meses.Select(m => (año, m.Mes)));
            }

            mesesConRegistros = mesesConRegistros.OrderByDescending(m => m.Año).ThenByDescending(m => m.Mes).ToList();

            _logger.LogInformation($"Se encontraron {mesesConRegistros.Count} meses con registros de asistencia.");

            while (mesesConRegistros.Count > 3)
            {
                var (añoEliminar, mesEliminar) = mesesConRegistros.Last();

                _logger.LogInformation($"Eliminando registros de asistencia del {mesEliminar}/{añoEliminar}...");

                await _asistenciaRepository.EliminarRegistrosPorMesAsync(añoEliminar, mesEliminar);
                mesesConRegistros.RemoveAt(mesesConRegistros.Count - 1);
            }

            _logger.LogInformation("Limpieza de registros de asistencia completada.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al limpiar registros de asistencia: {ex.Message}");
        }
    }
}