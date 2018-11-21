using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectorValueById : Specification<CollectorValue>
    {
        public Guid Id { get; }

        public CollectorValueById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<CollectorValue, bool>> ToExpression()
        {
            return collectorValue => collectorValue.Id == Id;
        }
    }
}