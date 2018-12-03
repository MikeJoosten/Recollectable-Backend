using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionCollectableById : Specification<CollectionCollectable>
    {
        public Guid Id { get; }

        public CollectionCollectableById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => collectable.Id == Id;
        }
    }
}