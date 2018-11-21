using LinqSpecs.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Specification<T> specification = null);
        Task<T> GetSingle(Specification<T> specification = null);
        Task Add(T item);
        void Update(T item);
        void Delete(T item);
        Task<bool> Exists(Specification<T> specification = null);
        Task<bool> Save();
    }
}