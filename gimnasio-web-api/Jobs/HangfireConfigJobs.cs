using Hangfire;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace gimnasio_web_api.Jobs
{
    public static class HangfireJobsConfig
    {
        public static void ConfigurarJobs()
        {
            var zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var opcionJob = new RecurringJobOptions
            {
                TimeZone = zonaHoraria
            };
            RecurringJob.AddOrUpdate<DatabaseBackupService>(
                "backup-job",
                service => service.HacerBackupAsync(),
                "0 19 * * *",
                opcionJob
            );

            RecurringJob.AddOrUpdate<AsistenciaCleanupService>(
                "CleanAsistencia-job",
                service => service.EjecutarLimpiezaAsync(),
                "0 19 1 * *",
                opcionJob
            );
        }
    }
}