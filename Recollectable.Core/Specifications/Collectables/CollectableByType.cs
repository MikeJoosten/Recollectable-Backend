using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    class CollectableByType : Specification<CollectionCollectable>
    {
        public string Type { get; }

        public CollectableByType(string type)
        {
            Type = type.Trim().ToLowerInvariant();
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => (collectable.Collectable as Currency).Type.ToLowerInvariant() == Type;
        }
    }
}
