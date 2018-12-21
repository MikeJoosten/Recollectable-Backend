using LinqSpecs.Core;
using Recollectable.Core.Entities.Collections;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collections
{
    class ConditionByLanguageCode : Specification<Condition>
    {
        public string Code { get; }

        public ConditionByLanguageCode(string code)
        {
            Code = code.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Condition, bool>> ToExpression()
        {
            return condition => condition.LanguageCode.ToLowerInvariant() == Code;
        }
    }
}
