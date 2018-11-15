using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<PagedList<Country>> Get(CountriesResourceParameters resourceParameters)
        {
            var countries = await _context.Countries.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CountryDto, Country>())
                .ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Name))
            {
                var name = resourceParameters.Name.Trim().ToLowerInvariant();
                countries = countries.Where(c => c.Name.ToLowerInvariant() == name).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                countries = countries.Where(c => c.Name.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<Country>.Create(countries,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<Country> GetById(Guid countryId)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == countryId);
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

        public async Task<bool> Exists(Guid countryId)
        {
            return await _context.Countries.AnyAsync(c => c.Id == countryId);
        }
    }
}