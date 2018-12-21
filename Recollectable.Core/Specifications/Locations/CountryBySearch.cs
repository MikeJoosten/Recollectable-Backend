using LinqSpecs.Core;
using Recollectable.Core.Entities.Locations;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Locations
{
    public class CountryBySearch : Specification<Country>
    {
        public string Search { get; }

        public CountryBySearch(string search)
        {
            Search = search.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Country, bool>> ToExpression()
        {
            return country => country.Name.ToLowerInvariant().Contains(Search);
        }
    }
}