using Microsoft.EntityFrameworkCore;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace gimnasio_web_api.Repositories
{
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly AppDbContext _context;

        public AsistenciaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AsistenciaDto?> ObtenerUsuarioInfoAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Codigo == id)
                .Select(u => new UsuarioDto
                {
                    Codigo = u.Codigo,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Foto = u.Foto
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return null;
            }

            var ultimoPagoYFecha = await ObtenerUltimaInformacionPagoAsync(id);

            return new AsistenciaDto
            {
                Usuario = usuario,
                UltimaFechaUsuario = ultimoPagoYFecha.UltimaFechaUsuario,
                UltimoPago = ultimoPagoYFecha.UltimoPago
            };
        }
        public async Task<AsistenciaDto> ObtenerUltimaInformacionPagoAsync(int usuarioId)
        {
            var ultimaFechaUsuario = await _context.Fechas_Usuarios
                .Where(fu => fu.UsuarioId == usuarioId)
                .OrderByDescending(fu => fu.FechaPago)
                .FirstOrDefaultAsync();

            var ultimoPago = await _context.Pagos
                .Where(p => p.CodigoUsuario == usuarioId)
                .OrderByDescending(p => p.FechaPago)
                .FirstOrDefaultAsync();

            if (ultimaFechaUsuario == null || ultimoPago == null)
            {
                return new AsistenciaDto();
            }

            var pagoDto = new PagoDto(
                ultimoPago.CodigoPago,
                ultimoPago.CodigoUsuario,
                ultimoPago.MesesPagados,
                ultimoPago.MesesPagadosA,
                ultimoPago.FechaPago,
                ultimoPago.Monto,
                ultimoPago.DetallePago,
                ultimoPago.IntervaloPago
            );

            return new AsistenciaDto
            {
                UltimaFechaUsuario = new FechasUsuarioDto
                {
                    FechaPago = ultimaFechaUsuario?.FechaPago,
                    FechaVencimiento = ultimaFechaUsuario?.FechaVencimiento
                },
                UltimoPago = pagoDto
            };
        }
        public async Task AddAsync(Asistencia asistencia)
        {
            await _context.Asistencias.AddAsync(asistencia);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            await _context.SaveChangesAsync();
        }
        public async Task <IEnumerable<Asistencia>>GetAsistenciaPorFechaAsync(DateTime primerafecha, DateTime? segundafecha = null)
        {
            var query = _context.Asistencias.AsQueryable();
            if (segundafecha.HasValue){
                query = query.Where(a => a.Fecha.Date >= primerafecha.Date && a.Fecha.Date <= segundafecha.Value.Date);
            }
            else
            {
                query = query.Where(a => a.Fecha.Date == primerafecha.Date);
            }
            return await query.OrderBy(a => a.Fecha).ToListAsync();
        }
        public async Task<IEnumerable<AsistenciaResumenDto>> GetAñosConAsistenciasAsync()
        {
            return await _context.Asistencias
                .GroupBy(a => a.Fecha.Year)
                .Select(g => new AsistenciaResumenDto
                {
                    Año = g.Key,
                    AsistenciasRegistradas = g.Count()
                })
                .OrderBy(r => r.Año)
                .ToListAsync();
        }

        public async Task<IEnumerable<AsistenciaResumenDto>> GetMesesConAsistenciasAsync(int year)
        {
            return await _context.Asistencias
                .Where(a => a.Fecha.Year == year)
                .GroupBy(a => a.Fecha.Month)
                .Select(g => new AsistenciaResumenDto
                {
                    Año = year,
                    Mes = g.Key,
                    AsistenciasRegistradas = g.Count()
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();
        }

        public async Task<IEnumerable<AsistenciaResumenDto>> GetDiasConAsistenciasAsync(int year, int month)
        {
            return await _context.Asistencias
                .Where(a => a.Fecha.Year == year && a.Fecha.Month == month)
                .GroupBy(a => a.Fecha.Day)
                .Select(g => new AsistenciaResumenDto
                {
                    Año = year,
                    Mes = month,
                    Dia = g.Key,
                    AsistenciasRegistradas = g.Count()
                })
                .OrderBy(r => r.Dia)
                .ToListAsync();
        }
        public async Task EliminarRegistrosPorMesAsync(int year, int month)
        {
            await _context.Asistencias
                .Where(a => a.Fecha.Year == year && a.Fecha.Month == month)
                .ExecuteDeleteAsync();
        }
    }
}