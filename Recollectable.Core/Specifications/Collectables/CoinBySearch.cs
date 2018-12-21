using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class CoinBySearch : Specification<Coin>
    {
        public string Search { get; }

        public CoinBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Coin, bool>> ToExpression()
        {
            return coin => coin.Type.ToLowerInvariant().Contains(Search)
                || coin.Country.Name.ToLowerInvariant().Contains(Search)
                || coin.ReleaseDate.ToLowerInvariant().Contains(Search)
                || coin.Metal.ToLowerInvariant().Contains(Search);
        }
    }
}