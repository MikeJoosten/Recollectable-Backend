using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private RecollectableContext _context;

        public CountryRepository(RecollectableContext context)
        {
            _context = context;
        }

        public IEnumerable<Country> GetCountries
            (CountriesResourceParameters resourceParameters)
        {
            var countries = _context.Countries
                .OrderBy(c => c.Name)
                .AsQueryable();

            if (!string.IsNullOrEmpty(resourceParameters.Name))
            {
                var name = resourceParameters.Name.Trim().ToLowerInvariant();
                countries = countries.Where(c => c.Name.ToLowerInvariant() == name);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                countries = countries.Where(c => c.Name.ToLowerInvariant().Contains(search));
            }

            return countries;
        }

        public Country GetCountry(Guid countryId)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == countryId);
        }

        public void AddCountry(Country country)
        {
            if (country.Id == Guid.Empty)
            {
                country.Id = Guid.NewGuid();
            }

            _context.Countries.Add(country);
        }

        public void UpdateCountry(Country country) { }

        public void DeleteCountry(Country country)
        {
            _context.Countries.Remove(country);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CountryExists(Guid countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }
    }
}