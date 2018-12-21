using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionCollectableByCountry : Specification<CollectionCollectable>
    {
        public string Country { get; }

        public CollectionCollectableByCountry(string country)
        {
            Country = country.Trim().ToLowerInvariant();
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectionCollectable => 
                collectionCollectable.Collectable.Country.Name.ToLowerInvariant() == Country;
        }
    }
}