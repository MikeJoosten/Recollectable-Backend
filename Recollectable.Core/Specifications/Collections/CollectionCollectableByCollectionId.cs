using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionCollectableByCollectionId : Specification<CollectionCollectable>
    {
        public Guid CollectionId { get; }

        public CollectionCollectableByCollectionId(Guid collectionId)
        {
            CollectionId = collectionId;
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectionCollectable => collectionCollectable.CollectionId == CollectionId;
        }
    }
}