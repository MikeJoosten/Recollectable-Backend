using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectableById : Specification<CollectionCollectable>
    {
        public Guid Id { get; }

        public CollectableById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => collectable.Id == Id;
        }
    }
}