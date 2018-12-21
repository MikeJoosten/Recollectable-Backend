using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICollectionCollectableService
    {
        Task<PagedList<CollectionCollectable>> FindCollectionCollectables
            (Guid collectionId, CollectionCollectablesResourceParameters resourceParameters);
        Task<CollectionCollectable> FindCollectionCollectableById(Guid collectionId, Guid id);
        Task<Collectable> FindCollectableById(Guid collectableId);
        Task CreateCollectionCollectable(CollectionCollectable collectable);
        void UpdateCollectionCollectable(CollectionCollectable collectable);
        void RemoveCollectionCollectable(CollectionCollectable collectable);
        Task<bool> CollectionCollectableExists(Guid collectionId, Guid id);
        Task<bool> Save();
    }
}