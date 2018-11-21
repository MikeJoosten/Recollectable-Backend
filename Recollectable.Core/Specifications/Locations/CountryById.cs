using LinqSpecs.Core;
using Recollectable.Core.Entities.Locations;
using System;
using System.Linq.Expressions;

namespace Recollectable.Core.Specifications.Locations
{
    public class CountryById : Specification<Country>
    {
        public Guid Id { get; }

        public CountryById(Guid id)
        {
            Id = id;
        }

        public override Expression<Func<Country, bool>> ToExpression()
        {
            return country => country.Id == Id;
        }
    }
}