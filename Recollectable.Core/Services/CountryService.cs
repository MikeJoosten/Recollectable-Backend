using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Locations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryService(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<PagedList<Country>> FindCountries(CountriesResourceParameters resourceParameters)
        {
            var countries = await _countryRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Name))
            {
                countries = await _countryRepository.GetAll(new CountryByName(resourceParameters.Name));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                countries = await _countryRepository.GetAll(new CountryBySearch(resourceParameters.Search));
            }

            countries = countries.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CountryPropertyMapping);

            return PagedList<Country>.Create(countries.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Country> FindCountryById(Guid id)
        {
            return await _countryRepository.GetSingle(new CountryById(id));
        }

        public async Task CreateCountry(Country country)
        {
            await _countryRepository.Add(country);
        }

        public void UpdateCountry(Country country)
        {
            _countryRepository.Update(country);
        }

        public void RemoveCountry(Country country)
        {
            _countryRepository.Delete(country);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _countryRepository.Exists(new CountryById(id));
        }

        public async Task<bool> Save()
        {
            return await _countryRepository.Save();
        }
    }
}