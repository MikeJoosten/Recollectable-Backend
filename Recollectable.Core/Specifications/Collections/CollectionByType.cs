using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionByType : Specification<Collection>
    {
        public string Type { get; }

        public CollectionByType(string type)
        {
            Type = type.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Collection, bool>> ToExpression()
        {
            return collection => collection.Type.ToLowerInvariant() == Type;
        }
    }
}