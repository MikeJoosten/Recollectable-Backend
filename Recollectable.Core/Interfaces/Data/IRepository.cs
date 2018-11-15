using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface IRepository<TEntity, TParameters>
        where TEntity : class
        where TParameters : class
    {
        Task<PagedList<TEntity>> Get(TParameters resourceParameters);
        Task<TEntity> GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> Exists(Guid id);
    }
}