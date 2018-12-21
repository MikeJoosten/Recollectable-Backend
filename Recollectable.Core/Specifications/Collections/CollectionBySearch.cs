using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionBySearch : Specification<Collection>
    {
        public string Search { get; }

        public CollectionBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Collection, bool>> ToExpression()
        {
            return collection => collection.Type.ToLowerInvariant().Contains(Search);
        }
    }
}