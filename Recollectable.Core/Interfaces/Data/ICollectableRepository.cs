using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICollectableRepository
    {
        Task<PagedList<CollectionCollectable>> GetCollectables(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        Task<CollectionCollectable> GetCollectableById(Guid collectionId, Guid Id);
        Task<Collectable> GetCollectableItem(Guid collectableItemId);
        void AddCollectable(CollectionCollectable collectable);
        void UpdateCollectable(CollectionCollectable collectable);
        void DeleteCollectable(CollectionCollectable collectable);
        Task<bool> Exists(Guid collectionId, Guid Id);
        Task<bool> Save();
    }
}