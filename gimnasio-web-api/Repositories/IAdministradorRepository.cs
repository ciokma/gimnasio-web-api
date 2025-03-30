using System.Threading.Tasks;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;

namespace gimnasio_web_api.Repositories
{
    public interface IAdministradorRepository: IRepository<Administrador, int>
    {
        Task<Administrador?> GetByUsernameAsync(string username);
    }
}