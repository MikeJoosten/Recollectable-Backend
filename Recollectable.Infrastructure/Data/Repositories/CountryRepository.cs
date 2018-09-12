using Recollectable.Core.DTOs.Locations;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CountryRepository : IRepository<Country, CountriesResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CountryRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Country> Get(CountriesResourceParameters resourceParameters)
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

        public Country GetById(Guid countryId)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == countryId);
        }

        public void Add(Country country)
        {
            if (country.Id == Guid.Empty)
            {
                country.Id = Guid.NewGuid();
            }

            _context.Countries.Add(country);
        }

        public void Update(Country country) { }

        public void Delete(Country country)
        {
            _context.Countries.Remove(country);
        }

        public bool Exists(Guid countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }
    }
}