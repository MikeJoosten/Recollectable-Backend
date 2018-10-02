using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;

namespace Recollectable.Core.Interfaces
{
    public interface ICollectableRepository
    {
        PagedList<CollectionCollectable> Get(Guid collectionId,
            CollectablesResourceParameters resourceParameters);
        CollectionCollectable GetById(Guid collectionId, Guid Id);
        Collectable GetCollectableItem(Guid collectableId);
        void Add(CollectionCollectable collectable);
        void Update(CollectionCollectable collectable);
        void Delete(CollectionCollectable collectable);
        bool Exists(Guid collectionId, Guid Id);
    }
}