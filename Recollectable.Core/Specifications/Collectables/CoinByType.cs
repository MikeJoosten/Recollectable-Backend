using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CoinByType : Specification<Coin>
    {
        public string Type { get; }

        public CoinByType(string type)
        {
            Type = type.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Coin, bool>> ToExpression()
        {
            return coin => coin.Type.ToLowerInvariant() == Type;
        }
    }
}