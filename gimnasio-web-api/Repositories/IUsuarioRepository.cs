using System.Threading.Tasks;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuarios, int>
    {
        Task<List<Usuarios>> GetActiveUsers();
    }
}