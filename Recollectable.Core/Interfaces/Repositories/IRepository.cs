using Recollectable.Core.Shared.Entities;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity, TParameters>
        where TEntity: class
        where TParameters: class
    {
        PagedList<TEntity> Get(TParameters resourceParameters);
        TEntity GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        bool Exists(Guid id);
    }
}