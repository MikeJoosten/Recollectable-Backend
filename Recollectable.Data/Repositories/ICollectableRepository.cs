using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICollectableRepository
    {
        PagedList<CollectionCollectable> GetCollectables(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        Collectable GetCollectable(Guid collectableId);
        CollectionCollectable GetCollectable(Guid collectionId, Guid Id);
        void AddCollectable(CollectionCollectable collectable);
        void UpdateCollectable(CollectionCollectable collectable);
        void DeleteCollectable(CollectionCollectable collectable);
        bool Save();
        bool CollectableExists(Guid Id);
    }
}