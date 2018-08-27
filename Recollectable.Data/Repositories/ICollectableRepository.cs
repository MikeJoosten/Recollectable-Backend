using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;

namespace Recollectable.Data.Repositories
{
    public interface ICollectableRepository
    {
        PagedList<CollectionCollectable> GetCollectables(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        Collectable GetCollectableItem(Guid collectableItemId);
        CollectionCollectable GetCollectable(Guid collectionId, Guid collectableId);
        void AddCollectable(CollectionCollectable collectable);
        void UpdateCollectable(CollectionCollectable collectable);
        void DeleteCollectable(CollectionCollectable collectable);
        bool Save();
        bool CollectableExists(Guid Id);
    }
}