using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionCollectableBySearch : Specification<CollectionCollectable>
    {
        public string Search { get; }

        public CollectionCollectableBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectionCollectable => 
                collectionCollectable.Collectable.Country.Name.ToLowerInvariant().Contains(Search) || 
                collectionCollectable.Collectable.ReleaseDate.ToLowerInvariant().Contains(Search);
        }
    }
}