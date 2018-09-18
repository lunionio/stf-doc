using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Infra.Cross.Generics
{
    public interface ICross<T> where T : class
    {
        Task<T> SaveAsync(T entity, string token);
        Task<T> UpdateAsync(T entity, string token);
        Task<IEnumerable<T>> GetAllAsync(string token);
        Task<T> GetByIdAsync(int entityId, int idCliente, string token);
        Task DeleteAsync(T entity, string token);
    }
}
