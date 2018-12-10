using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class ConditionBySearch : Specification<Condition>
    {
        public string Search { get; }

        public ConditionBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Condition, bool>> ToExpression()
        {
            return condition => condition.Grade.ToLowerInvariant().Contains(Search);
        }
    }
}