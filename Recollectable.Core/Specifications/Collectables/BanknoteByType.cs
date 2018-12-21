using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class BanknoteByType : Specification<Banknote>
    {
        public string Type { get; }

        public BanknoteByType(string type)
        {
            Type = type.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Banknote, bool>> ToExpression()
        {
            return banknote => banknote.Type.ToLowerInvariant() == Type;
        }
    }
}