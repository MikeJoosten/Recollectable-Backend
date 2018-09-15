using Recollectable.Core.Entities.Common;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Services;
using System;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public abstract class BaseRepository<TEntity, TParameters> : IRepository<TEntity, TParameters>
        where TEntity: class
        where TParameters: class
    {
        public readonly IPropertyMappingService _propertyMappingService;

        public BaseRepository()
        {
            _propertyMappingService = new PropertyMappingService();
        }

        abstract public PagedList<TEntity> Get(TParameters resourceParameters);
        abstract public TEntity GetById(Guid id);
        abstract public void Add(TEntity entity);
        abstract public void Update(TEntity entity);
        abstract public void Delete(TEntity entity);
        abstract public bool Exists(Guid id);
    }
}