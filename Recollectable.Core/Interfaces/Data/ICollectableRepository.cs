using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICollectableRepository
    {
        Task<PagedList<CollectionCollectable>> Get(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        Task<CollectionCollectable> GetById(Guid collectionId, Guid Id);
        Task<Collectable> GetCollectableItem(Guid collectableId);
        void Add(CollectionCollectable collectable);
        void Update(CollectionCollectable collectable);
        void Delete(CollectionCollectable collectable);
        Task<bool> Exists(Guid collectionId, Guid Id);
    }
}