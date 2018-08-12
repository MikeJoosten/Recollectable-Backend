using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICollectableRepository
    {
        IEnumerable<CollectionCollectable> GetCollectables(Guid collectionId);
        Collectable GetCollectable(Guid collectableId);
        CollectionCollectable GetCollectable(Guid collectionId, Guid Id);
        void AddCollectable(CollectionCollectable collectable);
        void UpdateCollectable(CollectionCollectable collectable);
        void DeleteCollectable(CollectionCollectable collectable);
        bool Save();
        bool CollectableExists(Guid Id);
    }
}