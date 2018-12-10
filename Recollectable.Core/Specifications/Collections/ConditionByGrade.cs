using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    public class ConditionByGrade : Specification<Condition>
    {
        public string Grade { get; }

        public ConditionByGrade(string grade)
        {
            Grade = grade.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Condition, bool>> ToExpression()
        {
            return condition => condition.Grade.ToLowerInvariant() == Grade;
        }
    }
}