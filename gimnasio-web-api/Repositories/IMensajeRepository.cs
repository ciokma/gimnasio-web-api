using System.Collections.Generic;
using System.Threading.Tasks;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Repositories
{
    public interface IMensajeRepository
    {
        Task<IEnumerable<Mensaje>> GetByEmisorSistemaCrossfitAsync();
        Task<IEnumerable<Mensaje>> GetAllExceptSistemaCrossfitAsync();
        Task<(int sistemaCount, List<int> sistemaIds, int usuarioCount, List<int> usuarioIds)> GetNumberMessagesAsync();
        Task MarkMessagesAsReadAsync(List<int> mensajeIds);
        Task<Mensaje> GetByIdAsync(int id);
        Task AddAsync(Mensaje entity);
        Task UpdateAsync(Mensaje entity);
        Task DeleteAsync(int id);
    }
}