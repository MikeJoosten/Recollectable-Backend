using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class BanknoteBySearch : Specification<Banknote>
    {
        public string Search { get; }

        public BanknoteBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Banknote, bool>> ToExpression()
        {
            return banknote => banknote.Type.ToLowerInvariant().Contains(Search)
                || banknote.Country.Name.ToLowerInvariant().Contains(Search)
                || banknote.ReleaseDate.ToLowerInvariant().Contains(Search)
                || banknote.Color.ToLowerInvariant().Contains(Search);
        }
    }
}