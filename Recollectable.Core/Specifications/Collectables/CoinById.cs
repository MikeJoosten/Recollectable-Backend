using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CoinById : Specification<Coin>
    {
        public Guid Id { get; }

        public CoinById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Coin, bool>> ToExpression()
        {
            return coin => coin.Id == Id;
        }
    }
}
