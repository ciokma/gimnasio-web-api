using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Repositories
{
    public interface IVentaRepository : IRepository<Venta, int>
    {
        Task<IEnumerable<DateTime>> GetFechasConVentasAsync();
        Task<IEnumerable<Venta>> GetVentasPorRangoFechasAsync(DateTime fechaInicio, DateTime? fechaFin = null);
        Task<Venta> GetVentaPorIdAsync(int id);
        Task AddVentaAsync(Venta venta);
        Task UpdateVentaAsync(Venta venta);
        Task DeleteVentaAsync(int id);
    }
}