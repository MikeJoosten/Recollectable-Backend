using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface ICollectableRepository
    {
        PagedList<CollectionCollectable> GetCollectables(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        CollectionCollectable GetCollectable(Guid collectionId, Guid collectableId);
        Collectable GetCollectableItem(Guid collectableItemId);
        void AddCollectable(CollectionCollectable collectable);
        void UpdateCollectable(CollectionCollectable collectable);
        void DeleteCollectable(CollectionCollectable collectable);
        bool Save();
        bool CollectableExists(Guid Id);
    }
}