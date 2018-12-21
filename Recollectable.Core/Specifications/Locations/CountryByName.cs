using LinqSpecs.Core;
using Recollectable.Core.Entities.Locations;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Locations
{
    public class CountryByName : Specification<Country>
    {
        public string Name { get; }

        public CountryByName(string name)
        {
            Name = name.Trim().ToLowerInvariant();
        }

        public override Expression<Func<Country, bool>> ToExpression()
        {
            return country => country.Name.ToLowerInvariant() == Name;
        }
    }
}