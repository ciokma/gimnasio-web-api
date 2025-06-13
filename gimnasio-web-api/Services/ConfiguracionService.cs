using gimnasio_web_api.Models;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace gimnasio_web_api.Services
{
    public class ConfiguracionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConfiguracionService> _logger;

        public ConfiguracionService(AppDbContext context, ILogger<ConfiguracionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task EjecutarConfiguracionesAsync()
        {
            _logger.LogInformation("Inicio de ejecución de configuraciones del sistema.");

            var configuraciones = await _context.ConfiguracionesSistema.ToListAsync();
            _logger.LogInformation("Se encontraron {Count} configuraciones.", configuraciones.Count);

            var configInactivar = configuraciones.FirstOrDefault(c => c.Id == 1 && c.Estado);
            var configEliminar = configuraciones.FirstOrDefault(c => c.Id == 2 && c.Estado);

            if (configInactivar != null && configInactivar.Valor.HasValue)
            {
                _logger.LogInformation("Configuración para inactivar usuarios activa. Valor: {Meses} meses.", configInactivar.Valor.Value);
                await InactivarUsuariosAsync(configInactivar.Valor.Value);
            }
            else
            {
                _logger.LogInformation("Configuración para inactivar usuarios no está activa o no tiene valor.");
            }

            if (configEliminar != null)
            {
                _logger.LogInformation("Configuración para eliminar usuarios inactivos activa.");
                await EliminarUsuariosInactivosAsync();
            }
            else
            {
                _logger.LogInformation("Configuración para eliminar usuarios inactivos no está activa.");
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cambios guardados en la base de datos.");
        }

        private async Task InactivarUsuariosAsync(int mesesInactividad)
        {
            var fechaLimite = DateTime.UtcNow.AddMonths(-mesesInactividad);
            _logger.LogInformation("Buscando usuarios con fecha de vencimiento anterior a {FechaLimite}.", fechaLimite.ToShortDateString());

            var usuariosConVencimiento = await _context.Fechas_Usuarios
                .Include(f => f.Usuario)
                .Where(f => f.Usuario != null && f.Usuario.Activo && f.FechaVencimiento != null)
                .GroupBy(f => f.UsuarioId)
                .Select(g => new
                {
                    Usuario = g.First().Usuario!,
                    UltimaFechaVencimiento = g.Max(f => f.FechaVencimiento)
                })
                .Where(x => x.UltimaFechaVencimiento <= fechaLimite)
                .ToListAsync();

            _logger.LogInformation("Se encontraron {Cantidad} usuarios para inactivar.", usuariosConVencimiento.Count);
        }

        private async Task EliminarUsuariosInactivosAsync()
        {
            var usuariosParaEliminar = await _context.Usuarios
                .Where(u => !u.Activo)
                .ToListAsync();

            _logger.LogInformation("Se encontraron {Cantidad} usuarios inactivos para eliminar.", usuariosParaEliminar.Count);

            _context.Usuarios.RemoveRange(usuariosParaEliminar);
        }

        public async Task<List<UsuariosInactivosDto>> ObtenerUsuariosInactivosAsync()
        {
            _logger.LogInformation("Consultando usuarios inactivos...");

            var usuariosInactivos = await _context.Fechas_Usuarios
                .Include(f => f.Usuario)
                .Where(f => f.Usuario != null && !f.Usuario.Activo)
                .GroupBy(f => f.UsuarioId)
                .Select(g => new UsuariosInactivosDto
                {
                    Codigo = g.First().Usuario!.Codigo,
                    Nombres = g.First().Usuario!.Nombres,
                    Apellidos = g.First().Usuario!.Apellidos,
                    Foto = g.First().Usuario!.Foto,
                    Activo = g.First().Usuario!.Activo,
                    FechaPago = g.Max(f => f.FechaPago),
                    FechaVencimiento = g.Max(f => f.FechaVencimiento)
                })
                .ToListAsync();

            _logger.LogInformation("Usuarios inactivos encontrados: {Cantidad}", usuariosInactivos.Count);

            return usuariosInactivos;
        }
    }
}