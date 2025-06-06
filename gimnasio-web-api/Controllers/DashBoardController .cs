using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class DashBoardController  : ControllerBase
{
    private readonly IUsuarioRepository _repository;
    private readonly AppDbContext _context;
    private readonly IVentaRepository _ventaRepository;
    private readonly IAsistenciaRepository _asistenciaRepository;
    public DashBoardController(IUsuarioRepository repository, AppDbContext context, IVentaRepository ventaRepository, IAsistenciaRepository asistenciaRepository)
    {
        _repository = repository;
        _context = context;
        _ventaRepository = ventaRepository;
        _asistenciaRepository = asistenciaRepository;

    }


    [HttpGet]
    public async Task<ActionResult<DashboardResumenDto>> GetDashBoardInfo()
    {
        var usuarios = await _repository.GetActiveUsers();
        var pagosAyer =  await totalPaymentYesterdayAsync();
        var pagosHoy = await totalPaymentTodayAsync();
        var totalVentasHoy = await totalSalesTodayAsync();
        var totalVentasAyer = await totalSalesYesterdayAsync();
        var asistenciaHoy = await totalAttendanceTodayAsync();
        var asistenciaAyer = await totalAttendanceYesterdayAsync();
        // Este es el objeto dummy que luego tú puedes popular dinámicamente
        var resumen = new DashboardResumenDto
        {
            Usuarios = usuarios.Count,
            UsuariosDiff = 0,
            Ingresos = pagosHoy,
            IngresosDiff = pagosAyer,
            Ventas = totalVentasHoy,
            VentasDiff = totalVentasAyer,
            Asistencias = asistenciaHoy,
            AsistenciasDiff = asistenciaAyer
        };

        return Ok(resumen);
    }
    private async Task<decimal> totalPaymentYesterdayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today.AddDays(-1); // Ayer 00:00
            DateTime endDate = DateTime.Today;               // Hoy 00:00

            decimal pagosAyer = await _context.Pagos
                .Where(p => p.FechaPago >= startDate && p.FechaPago < endDate)
                .SumAsync(p => p.Monto);

            return pagosAyer;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en totalPaymentYesterdayAsync: {ex.Message}");
            return 0m;
        }
    }

    private async Task<decimal> totalPaymentTodayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today;                   // Hoy 00:00
            DateTime endDate = DateTime.Today.AddDays(1);          // Mañana 00:00

            decimal pagosHoy = await _context.Pagos
                .Where(p => p.FechaPago >= startDate && p.FechaPago < endDate)
                .SumAsync(p => p.Monto);

            return pagosHoy;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en totalPaymentTodayAsync: {ex.Message}");
            return 0m;
        }
    }
    private async Task<decimal> totalSalesTodayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today;                   // Hoy 00:00
            DateTime endDate = DateTime.Today.AddDays(1);          // Mañana 00:00

            var ventas = await _ventaRepository
                           .GetVentasPorRangoFechasAsync(startDate, endDate);
            decimal total = ventas.ToList().Count;

          
            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al calcular total de ventas hoy: {ex.Message}");
            return 0m;
        }
    }
    private async Task<decimal> totalSalesYesterdayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today.AddDays(-1); // Ayer 00:00
            DateTime endDate = DateTime.Today;

            var ventas = await _ventaRepository
                           .GetVentasPorRangoFechasAsync(startDate, endDate);
            decimal total = ventas.ToList().Count;  

          
            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al calcular total de ventas ayer: {ex.Message}");
            return 0m;
        }
    }
    private async Task<int> totalAttendanceTodayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today;                   // Hoy 00:00
            DateTime endDate = DateTime.Today.AddDays(1);          // Mañana 00:00


            var asistencias = await _asistenciaRepository.GetAsistenciaPorFechaAsync(startDate, endDate);
            int total = asistencias.ToList().Count;


            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al calcular total de ventas ayer: {ex.Message}");
            return 0;
        }
    }
    private async Task<int> totalAttendanceYesterdayAsync()
    {
        try
        {
            DateTime startDate = DateTime.Today.AddDays(-1); // Ayer 00:00
            DateTime endDate = DateTime.Today;



            var asistencias = await _asistenciaRepository.GetAsistenciaPorFechaAsync(startDate, endDate);
            int total = asistencias.ToList().Count;


            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al calcular total de ventas ayer: {ex.Message}");
            return 0;
        }
    }

}