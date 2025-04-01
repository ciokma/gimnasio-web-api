
using System.Diagnostics;
using gimnasio_web_api.Repositories;
using gimnasio_web_api.Models;
using MySqlConnector;

public class DatabaseBackupService
{
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _backupPath = "C:\\Backups";
    private readonly IBackupRepository _backupRepository;
    private readonly IConfiguration _configuration;

    public DatabaseBackupService(ILogger<DatabaseBackupService> logger, IConfiguration configuration, IBackupRepository backupRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _backupRepository = backupRepository;
    }

    public async Task HacerBackupAsync()
    {
        try
        {
            var backupConfig = await _backupRepository.GetBackupConfigAsync();

            if (backupConfig == null)
            {
                _logger.LogWarning("No hay configuración de respaldo en la base de datos. Se ejecutará el primer backup ahora.");

                await EjecutarBackupAsync();

                backupConfig = new Backup
                {
                    FechaRespaldoAnterior = DateTime.UtcNow.Date,
                    FrecuenciaRespaldo = "mes",
                    ProximoRespaldo = CalcularProximoRespaldo(DateTime.UtcNow, "mes")
                };

                await _backupRepository.AddBackupConfigAsync(backupConfig);
                _logger.LogInformation("Nueva configuración de respaldo guardada en la base de datos.");
            }
            else
            {
                if (DateTime.UtcNow.Date >= backupConfig.ProximoRespaldo)
                {
                    await EjecutarBackupAsync();

                    backupConfig.FechaRespaldoAnterior = DateTime.UtcNow.Date;
                    backupConfig.ProximoRespaldo = CalcularProximoRespaldo(DateTime.UtcNow, backupConfig.FrecuenciaRespaldo);

                    await _backupRepository.UpdateBackupConfigAsync(backupConfig);
                    _logger.LogInformation("Configuración de respaldo actualizada después del backup.");
                }
                else
                {
                    _logger.LogInformation("Aún no ha pasado el tiempo para el próximo respaldo.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al ejecutar el backup: {ex.Message}");
        }
    }

    private async Task EjecutarBackupAsync()
    {
        try
        {
            Directory.CreateDirectory(_backupPath);

            var connectionString = _configuration.GetConnectionString("AppDbContext");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string is null or empty.");
            }
            var builder = new MySqlConnectionStringBuilder(connectionString);
            string databaseName = builder.Database;
            string user = builder.UserID;
            string password = builder.Password;
            string server = builder.Server;

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string fileName = Path.Combine(_backupPath, $"backup_{databaseName}_{timestamp}.sql");

            string command = $"mysqldump --user={user} --password={password} --host={server} {databaseName} > \"{fileName}\"";

            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        _logger.LogInformation($"Backup realizado correctamente en: {fileName}");
                    }
                    else
                    {
                        string error = await process.StandardError.ReadToEndAsync();
                        _logger.LogError($"Error al realizar el backup: {error}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al ejecutar el backup: {ex.Message}");
        }
    }
    private DateTime CalcularProximoRespaldo(DateTime fechaActual, string frecuencia)
    {
        switch (frecuencia.ToLower())
        {
            case "dia":
                return fechaActual.AddDays(1).Date;
            case "semana":
                return fechaActual.AddDays(7).Date;
            case "quincena":
                return fechaActual.AddDays(15).Date;
            case "mes":
                return fechaActual.AddMonths(1).Date;
            case "año":
                return fechaActual.AddYears(1).Date;
            default:
                _logger.LogWarning($"Frecuencia desconocida: {frecuencia}. Se usará un mes por defecto.");
                return fechaActual.AddMonths(1).Date;
        }
    }
}