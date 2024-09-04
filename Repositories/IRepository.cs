using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gimnasio_web_api.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);


        
    }

}