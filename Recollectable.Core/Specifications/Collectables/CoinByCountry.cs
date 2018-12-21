using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CoinByCountry : Specification<Coin>
    {
        public string Country { get; }

        public CoinByCountry(string country)
        {
            Country = country.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Coin, bool>> ToExpression()
        {
            return coin => coin.Country.Name.ToLowerInvariant() == Country;
        }
    }
}