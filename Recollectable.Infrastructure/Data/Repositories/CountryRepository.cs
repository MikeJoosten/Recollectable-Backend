using Recollectable.Core.DTOs.Locations;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CountryRepository : BaseRepository<Country, CountriesResourceParameters>
    {
        private RecollectableContext _context;

        public CountryRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<Country> Get(CountriesResourceParameters resourceParameters)
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

        public override Country GetById(Guid countryId)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == countryId);
        }

        public override void Add(Country country)
        {
            if (country.Id == Guid.Empty)
            {
                country.Id = Guid.NewGuid();
            }

            _context.Countries.Add(country);
        }

        public override void Update(Country country) { }

        public override void Delete(Country country)
        {
            _context.Countries.Remove(country);
        }

        public override bool Exists(Guid countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }
    }
}