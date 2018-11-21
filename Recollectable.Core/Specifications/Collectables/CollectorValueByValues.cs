using LinqSpecs.Core;
using Recollectable.Core.Comparers;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CollectorValueByValues : Specification<CollectorValue>
    {
        public CollectorValue CollectorValue { get; }

        public CollectorValueByValues(CollectorValue collectorValue)
        {
            CollectorValue = collectorValue;
        }

        public override Expression<Func<CollectorValue, bool>> ToExpression()
        {
            return collectorValue => new CollectorValueComparer().Equals(collectorValue, CollectorValue);
        }
    }
}