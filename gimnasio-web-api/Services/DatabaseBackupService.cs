
using System.Diagnostics;
using gimnasio_web_api.Repositories;
using gimnasio_web_api.Models;
using MySqlConnector;
using dotenv.net;

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
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "undefined";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "undefined";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "undefined";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

        try
        {
            var backupPath = Environment.GetEnvironmentVariable("BACKUP_PATH") ??
                            (OperatingSystem.IsWindows() ? "C:\\Backups" : "/var/backups");

            Directory.CreateDirectory(backupPath);

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string fileName = Path.Combine(backupPath, $"backup_{dbName}_{timestamp}.sql");

            string dumpCommand = $"mysqldump --user={dbUser} --password={dbPassword} --host={dbHost} {dbName}";

            string fullCommand;
            ProcessStartInfo processInfo;

            if (OperatingSystem.IsWindows())
            {
                fullCommand = $"{dumpCommand} > \"{fileName}\"";
                processInfo = new ProcessStartInfo("cmd.exe", "/c " + fullCommand);
            }
            else
            {
                fullCommand = $"{dumpCommand} > '{fileName}'";
                processInfo = new ProcessStartInfo("/bin/bash", "-c \"" + fullCommand + "\"");
            }

            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

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