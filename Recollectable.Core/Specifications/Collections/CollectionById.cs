using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class CollectionById : Specification<Collection>
    {
        public Guid Id { get; }

        public CollectionById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Collection, bool>> ToExpression()
        {
            return collection => collection.Id == Id;
        }
    }
}