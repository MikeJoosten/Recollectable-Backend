using LinqSpecs.Core;
using Recollectable.Core.Entities.Collectables;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Collectables
{
    public class BanknoteByCountry : Specification<Banknote>
    {
        public string Country { get; }

        public BanknoteByCountry(string country)
        {
            Country = country.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Banknote, bool>> ToExpression()
        {
            return banknote => banknote.Country.Name.ToLowerInvariant() == Country;
        }
    }
}