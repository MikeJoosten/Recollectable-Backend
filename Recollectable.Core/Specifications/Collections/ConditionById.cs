using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class ConditionById : Specification<Condition>
    {
        public Guid Id { get; }

        public ConditionById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Condition, bool>> ToExpression()
        {
            return condition => condition.Id == Id;
        }
    }
}