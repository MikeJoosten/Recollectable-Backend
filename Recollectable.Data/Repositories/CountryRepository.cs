using Recollectable.Data.Helpers;
using Recollectable.Data.Services;
using Recollectable.Domain.Entities;
using Recollectable.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Data.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CountryRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Country> GetCountries
            (CountriesResourceParameters resourceParameters)
        {
            var countries = _context.Countries.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CountryDto, Country>());

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

            return PagedList<Country>.Create(countries,
                resourceParameters.Page,
                resourceParameters.PageSize);
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