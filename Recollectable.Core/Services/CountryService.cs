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
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Country>> FindCountries(CountriesResourceParameters resourceParameters)
        {
            var countries = await _unitOfWork.Countries.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Name))
            {
                countries = await _unitOfWork.Countries.GetAll(new CountryByName(resourceParameters.Name));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                countries = await _unitOfWork.Countries.GetAll(new CountryBySearch(resourceParameters.Search));
            }

            countries = countries.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CountryPropertyMapping);

            return PagedList<Country>.Create(countries.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Country> FindCountryById(Guid id)
        {
            return await _unitOfWork.Countries.GetSingle(new CountryById(id));
        }

        public async Task CreateCountry(Country country)
        {
            await _unitOfWork.Countries.Add(country);
        }

        public void UpdateCountry(Country country) { }

        public void RemoveCountry(Country country)
        {
            _unitOfWork.Countries.Delete(country);
        }

        public async Task<bool> Exists(Guid id)
        {
            var country = await _unitOfWork.Countries.GetSingle(new CountryById(id));
            return country != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}