using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectableBySearch : Specification<CollectionCollectable>
    {
        public string Search { get; }

        public CollectableBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => (collectable.Collectable as Currency).Type.ToLowerInvariant().Contains(Search)
                || collectable.Collectable.Country.Name.ToLowerInvariant().Contains(Search)
                || collectable.Collectable.ReleaseDate.ToLowerInvariant().Contains(Search);
        }
    }
}
