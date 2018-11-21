using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectableByCollectionId : Specification<CollectionCollectable>
    {
        public Guid CollectionId { get; }

        public CollectableByCollectionId(Guid collectionId)
        {
            CollectionId = collectionId;
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => collectable.CollectionId == CollectionId;
        }
    }
}
