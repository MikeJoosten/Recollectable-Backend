using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectableByCountry : Specification<CollectionCollectable>
    {
        public string Country { get; }

        public CollectableByCountry(string country)
        {
            Country = country.Trim().ToLowerInvariant();
        }

        public override Expression<Func<CollectionCollectable, bool>> ToExpression()
        {
            return collectable => collectable.Collectable.Country.Name.ToLowerInvariant() == Country;
        }
    }
}